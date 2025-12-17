
#include "Unreal2dDungeonMicroserviceClients/Public/AutoGen/ObtainItemsView.h"
#include "Serialization/BeamJsonUtils.h"




void UObtainItemsView::BeamSerializeProperties(TUnrealJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("contentId"), ContentId, Serializer);
	UBeamJsonUtils::SerializeArray<UItemPropertyView*>(TEXT("properties"), Properties, Serializer);
}

void UObtainItemsView::BeamSerializeProperties(TUnrealPrettyJsonSerializer& Serializer) const
{
	UBeamJsonUtils::SerializeRawPrimitive(TEXT("contentId"), ContentId, Serializer);
	UBeamJsonUtils::SerializeArray<UItemPropertyView*>(TEXT("properties"), Properties, Serializer);		
}

void UObtainItemsView::BeamDeserializeProperties(const TSharedPtr<FJsonObject>& Bag)
{
	UBeamJsonUtils::DeserializeRawPrimitive(TEXT("contentId"), Bag, ContentId);
	UBeamJsonUtils::DeserializeArray<UItemPropertyView*>(TEXT("properties"), Bag, Properties, OuterOwner);
}



