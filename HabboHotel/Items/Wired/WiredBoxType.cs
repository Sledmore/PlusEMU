using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plus.HabboHotel.Items.Wired
{
    public enum WiredBoxType
    {
        None,
        TriggerRoomEnter,
        TriggerUserSays,
        TriggerRepeat,
        TriggerStateChanges,
        TriggerWalkOnFurni,
        TriggerWalkOffFurni,
        TriggerGameStarts,
        TriggerGameEnds,
        TriggerUserFurniCollision,
        TriggerUserSaysCommand,

        EffectShowMessage,
        EffectTeleportToFurni,
        EffectToggleFurniState,
        EffectKickUser,
        EffectMatchPosition,
        EffectMoveAndRotate,
        EffectMoveFurniToNearestUser,
        EffectMoveFurniFromNearestUser,
        EffectMuteTriggerer,
        EffectGiveReward,
        EffectExecuteWiredStacks,

        EffectTeleportBotToFurniBox,
        EffectBotChangesClothesBox,
        EffectBotMovesToFurniBox,
        EffectBotCommunicatesToAllBox,
        EffectBotCommunicatesToUserBox,
        EffectBotFollowsUserBox,
        EffectBotGivesHanditemBox,

        EffectAddActorToTeam,
        EffectRemoveActorFromTeam,
        EffectSetRollerSpeed,
        EffectRegenerateMaps,
        EffectGiveUserBadge,

        ConditionFurniHasUsers,
        ConditionFurniHasFurni,
        ConditionTriggererOnFurni,
        ConditionIsGroupMember,
        ConditionIsNotGroupMember,
        ConditionTriggererNotOnFurni,
        ConditionFurniHasNoUsers,
        ConditionIsWearingBadge,
        ConditionIsWearingFX,
        ConditionIsNotWearingBadge,
        ConditionIsNotWearingFX,
        ConditionMatchStateAndPosition,
        ConditionDontMatchStateAndPosition,
        ConditionUserCountInRoom,
        ConditionUserCountDoesntInRoom,
        ConditionFurniTypeMatches,
        ConditionFurniTypeDoesntMatch,
        ConditionFurniHasNoFurni,
        ConditionActorHasHandItemBox,
        ConditionActorIsInTeamBox,

        AddonRandomEffect
    }
}