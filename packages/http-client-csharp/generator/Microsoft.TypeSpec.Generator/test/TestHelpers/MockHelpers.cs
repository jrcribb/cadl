// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Primitives;
using Microsoft.TypeSpec.Generator.Providers;
using Microsoft.TypeSpec.Generator.SourceInput;
using Microsoft.TypeSpec.Generator.Tests.Common;
using Moq;
using Moq.Protected;

namespace Microsoft.TypeSpec.Generator.Tests
{
    internal static class MockHelpers
    {
        public const string TestHelpersFolder = "TestHelpers";

        public async static Task<Mock<CodeModelPlugin>> LoadMockPluginAsync(
            Func<InputType, CSharpType?>? createCSharpTypeCore = null,
            Func<InputModelType, ModelProvider?>? createModelCore = null,
            Func<InputEnumType, TypeProvider?, EnumProvider?>? createEnumCore = null,
            Func<OutputLibrary>? createOutputLibrary = null,
            string? configuration = null,
            InputModelType[]? inputModelTypes = null,
            InputEnumType[]? inputEnumTypes = null,
            Func<Task<Compilation>>? compilation = null,
            IEnumerable<MetadataReference>? additionalMetadataReferences = null,
            IEnumerable<string>? sharedSourceDirectories = null,
            IEnumerable<string>? typesToKeep = null)
        {
            var mockPlugin = LoadMockPlugin(
                createCSharpTypeCore,
                createModelCore,
                createEnumCore,
                createOutputLibrary,
                configuration,
                inputModelTypes,
                inputEnumTypes,
                additionalMetadataReferences,
                sharedSourceDirectories,
                typesToKeep);

            var compilationResult = compilation == null ? null : await compilation();

            var sourceInputModel = new Mock<SourceInputModel>(() => new SourceInputModel(compilationResult)) { CallBase = true };
            mockPlugin.Setup(p => p.SourceInputModel).Returns(sourceInputModel.Object);

            return mockPlugin;
        }

        public static Mock<CodeModelPlugin> LoadMockPlugin(
            Func<InputType, CSharpType?>? createCSharpTypeCore = null,
            Func<InputModelType, ModelProvider?>? createModelCore = null,
            Func<InputEnumType, TypeProvider?, EnumProvider?>? createEnumCore = null,
            Func<OutputLibrary>? createOutputLibrary = null,
            string? configuration = null,
            InputModelType[]? inputModelTypes = null,
            InputEnumType[]? inputEnumTypes = null,
            IEnumerable<MetadataReference>? additionalMetadataReferences = null,
            IEnumerable<string>? sharedSourceDirectories = null,
            IEnumerable<string>? typesToKeep = null)
        {
            var configFilePath = Path.Combine(AppContext.BaseDirectory, TestHelpersFolder);
            // initialize the singleton instance of the plugin
            var mockPlugin = new Mock<CodeModelPlugin>(new GeneratorContext(Configuration.Load(configFilePath, configuration))) { CallBase = true };

            var mockTypeFactory = new Mock<TypeFactory>() { CallBase = true };

            if (createCSharpTypeCore != null)
            {
                mockTypeFactory.Protected().Setup<CSharpType?>("CreateCSharpTypeCore", ItExpr.IsAny<InputType>()).Returns((InputType inputType) => createCSharpTypeCore.Invoke(inputType));
            }

            if (createModelCore != null)
            {
                mockTypeFactory.Protected().Setup<ModelProvider?>("CreateModelCore", ItExpr.IsAny<InputModelType>()).Returns((InputModelType inputModel) => createModelCore.Invoke(inputModel));
            }

            if (createEnumCore != null)
            {
                mockTypeFactory.Protected().Setup<EnumProvider?>("CreateEnumCore", ItExpr.IsAny<InputEnumType>(), ItExpr.IsAny<TypeProvider?>()).Returns((InputEnumType inputEnum, TypeProvider? type) => createEnumCore.Invoke(inputEnum, type));
            }

            if (createOutputLibrary != null)
            {
                mockPlugin.Setup(p => p.OutputLibrary).Returns(createOutputLibrary);
            }

            Mock<InputLibrary> mockInputLibrary = new Mock<InputLibrary>() { CallBase = true };
            mockInputLibrary.Setup(l => l.InputNamespace).Returns(InputFactory.Namespace(
                "Sample",
                models: inputModelTypes,
                enums: inputEnumTypes));

            mockPlugin.Setup(p => p.InputLibrary).Returns(mockInputLibrary.Object);

            mockPlugin.SetupGet(p => p.TypeFactory).Returns(mockTypeFactory.Object);

            var sourceInputModel = new Mock<SourceInputModel>(() => new SourceInputModel(null)) { CallBase = true };
            mockPlugin.Setup(p => p.SourceInputModel).Returns(sourceInputModel.Object);

            if (additionalMetadataReferences != null)
            {
                foreach (var reference in additionalMetadataReferences)
                {
                    mockPlugin.Object.AddMetadataReference(reference);
                }
            }

            if (sharedSourceDirectories != null)
            {
                foreach (var directory in sharedSourceDirectories)
                {
                    mockPlugin.Object.AddSharedSourceDirectory(directory);
                }
            }

            if (typesToKeep != null)
            {
                foreach (var type in typesToKeep)
                {
                    mockPlugin.Object.AddTypeToKeep(type);
                }
            }

            CodeModelPlugin.Instance = mockPlugin.Object;

            return mockPlugin;
        }
    }
}
