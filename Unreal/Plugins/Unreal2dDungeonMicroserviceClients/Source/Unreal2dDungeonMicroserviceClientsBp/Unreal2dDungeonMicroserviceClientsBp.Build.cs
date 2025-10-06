// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class Unreal2dDungeonMicroserviceClientsBp : ModuleRules
{
	public Unreal2dDungeonMicroserviceClientsBp(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = ModuleRules.PCHUsageMode.UseExplicitOrSharedPCHs;

		PublicDependencyModuleNames.AddRange(
			new string[]
			{
				"Core",
				"Unreal2dDungeonMicroserviceClients",

				"BeamableCore",
                "BeamableCoreRuntime",
                "BeamableCoreBlueprintNodes",
                
                "BlueprintGraph",
			});


		PrivateDependencyModuleNames.AddRange(
			new string[]
			{
				"CoreUObject",
				"Engine",
				"Slate",
				"SlateCore",					
			});
	}

	public static void AddMicroserviceClientsBp(ModuleRules Rules)
	{
		Rules.PublicDependencyModuleNames.AddRange(new[] { "Unreal2dDungeonMicroserviceClientsBp" });
	}
	
}