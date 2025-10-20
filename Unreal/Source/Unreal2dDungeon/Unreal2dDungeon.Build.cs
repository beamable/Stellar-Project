// Copyright Epic Games, Inc. All Rights Reserved.

using UnrealBuildTool;

public class Unreal2dDungeon : ModuleRules
{
	public Unreal2dDungeon(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
	
		PublicDependencyModuleNames.AddRange([
			"Core", "CoreUObject", "Engine", "InputCore", "EnhancedInput", "BeamableCore",
			"HTTP"
		]);

		PrivateDependencyModuleNames.AddRange([
			"CoreUObject",
			"Engine",
			"Slate",
			"SlateCore",
			"HTTP"
		]);

		

		// To include OnlineSubsystemSteam, add it to the plugins section in your uproject file with the Enabled attribute set to true
		
		
		Unreal2dDungeonMicroserviceClients.AddMicroserviceClients(this);
		Beam.AddRuntimeModuleDependencies(this);
	}
}
