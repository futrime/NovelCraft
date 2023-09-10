using Newtonsoft.Json;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using System;

namespace NovelCraft.Utilities.Messages
{


    /// <summary>
    /// Parses messages from JSON.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses a message from JSON.
        /// </summary>
        public static IMessage Parse(JToken json)
        {
            return Parse(json.ToString()!);
        }

        /// <summary>
        /// Parses a message from JSON.
        /// </summary>
        public static IMessage Parse(string jsonString)
        {
            var result = JsonConvert.DeserializeObject<EmptyMessage>(jsonString)!;

            IMessage.BoundToKind boundTo = (IMessage.BoundToKind)(int)result.BoundTo!;
            IMessage.MessageKind kind = (IMessage.MessageKind)(int)result.Type!;

            return (boundTo, kind) switch
            {
                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.Ping) =>
                  JsonConvert.DeserializeObject<ClientPingMessage>(jsonString)!,
                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.Ping) =>
                  JsonConvert.DeserializeObject<ServerPongMessage>(jsonString)!,


                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.Error) =>
                  JsonConvert.DeserializeObject<ErrorMessage>(jsonString)!,


                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterBlockChange) =>
                  JsonConvert.DeserializeObject<ServerAfterBlockChangeMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityAttack) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityAttackMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityCreate) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityCreateMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityDespawn) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityDespawnMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityHurt) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityHurtMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityOrientationChange) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityOrientationChangeMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityPositionChange) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityPositionChangeMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntityRemove) =>
                  JsonConvert.DeserializeObject<ServerAfterEntityRemoveMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterEntitySpawn) =>
                  JsonConvert.DeserializeObject<ServerAfterEntitySpawnMessage>(jsonString)!,

                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.AfterPlayerInventoryChange) =>
                  JsonConvert.DeserializeObject<ServerAfterPlayerInventoryChangeMessage>(jsonString)!,


                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetBlocksAndEntities) =>
                  JsonConvert.DeserializeObject<ClientGetBlocksAndEntitiesMessage>(jsonString)!,
                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetBlocksAndEntities) =>
                  JsonConvert.DeserializeObject<ServerGetBlocksAndEntitiesMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetPlayerInfo) =>
                  JsonConvert.DeserializeObject<ClientGetPlayerInfoMessage>(jsonString)!,
                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetPlayerInfo) =>
                  JsonConvert.DeserializeObject<ServerGetPlayerInfoMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.GetTick) =>
                  JsonConvert.DeserializeObject<ClientGetTickMessage>(jsonString)!,
                (IMessage.BoundToKind.ClientBound, IMessage.MessageKind.GetTick) =>
                  JsonConvert.DeserializeObject<ServerGetTickMessage>(jsonString)!,


                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformAttack) =>
                  JsonConvert.DeserializeObject<ClientPerformAttackMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformCraft) =>
                  JsonConvert.DeserializeObject<ClientPerformCraftMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformDropItem) =>
                  JsonConvert.DeserializeObject<ClientPerformDropItemMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformJump) =>
                  JsonConvert.DeserializeObject<ClientPerformJumpMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformLookAt) =>
                  JsonConvert.DeserializeObject<ClientPerformLookAtMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformMergeSlots) =>
                  JsonConvert.DeserializeObject<ClientPerformMergeSlotsMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformMove) =>
                  JsonConvert.DeserializeObject<ClientPerformMoveMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformRotate) =>
                  JsonConvert.DeserializeObject<ClientPerformRotateMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformSwapSlots) =>
                  JsonConvert.DeserializeObject<ClientPerformSwapSlotsMessage>(jsonString)!,


                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformSwitchMainHandSlot) =>
                  JsonConvert.DeserializeObject<ClientPerformSwitchMainHandSlotMessage>(jsonString)!,

                (IMessage.BoundToKind.ServerBound, IMessage.MessageKind.PerformUse) =>
                  JsonConvert.DeserializeObject<ClientPerformUseMessage>(jsonString)!,

                _ => throw new Exception("The message is not supported"),
            };
        }
    }
}