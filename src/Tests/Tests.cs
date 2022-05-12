using AssemblyToReference;
using Mono.Cecil;
using Mono.Cecil.Cil;
using VerifyTests.Http;

[TestFixture]
public class Tests
{
    [Test]
    public async Task Simple()
    {
        var solutionDirectory = AttributeReader.GetSolutionDirectory();

        var assemblyToProcessPath = Path.Combine(solutionDirectory, "AssemblyToProcess/bin/Debug/net6.0/AssemblyToProcess.dll");
        var assemblyToReferencePath = Path.Combine(solutionDirectory, "AssemblyToReference/bin/Debug/net6.0/AssemblyToReference.dll");
        var targetPath = Path.Combine(solutionDirectory, "AssemblyToProcess/bin/Debug/net6.0/AssemblyToProcess2.dll");
        using var resolver = new AssemblyResolver(new []{assemblyToReferencePath});
        var (moduleToReference, _) = ModuleReaderWriter.Read(assemblyToReferencePath, resolver);
        var (moduleToProcess, hasSymbols) = ModuleReaderWriter.Read(assemblyToProcessPath, resolver);
        moduleToProcess.ModuleReferences.Add(moduleToReference);
        var sentryHttpClient = moduleToProcess.ImportReference(moduleToReference.Types.Single(x => x.Name == "SentryHttpClient"));
        foreach (var type in moduleToProcess.GetTypes())
        {
            foreach (var method in type.Methods)
            {
                var instructions = method.Body.Instructions;
                foreach (var instruction in instructions.Where(x => x.OpCode == OpCodes.Newobj))
                {
                    if (instruction.Operand is not MethodReference reference)
                    {
                        continue;
                    }

                    var referenceDeclaringType = reference.DeclaringType;
                    if (reference.Name == ".ctor" &&
                        referenceDeclaringType.FullName == "System.Net.Http.HttpClient")
                    {
                        reference.DeclaringType = sentryHttpClient;
                    }
                }
            }
        }

        ModuleReaderWriter.Write(null, hasSymbols, moduleToProcess, targetPath);

        var assembly = Assembly.LoadFrom(targetPath);
        var classToTestType = assembly.GetType("ClassToTest")!;
        HttpRecording.StartRecording();
        await (Task) classToTestType.GetMethod("MethodWithHttpClient")!.Invoke(null, null)!;
        await (Task) classToTestType.GetMethod("MethodWithHttpClientAndHandler")!.Invoke(null, null)!;
        await Verify(new
        {
            SentryHttpClientHandler.SendWasCalled
        });
    }
}