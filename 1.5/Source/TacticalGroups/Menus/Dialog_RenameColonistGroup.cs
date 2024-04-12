using UnityEngine;
using Verse;
using Verse.Sound;

namespace TacticalGroups
{
	public class Dialog_RenameColonistGroup : Dialog_Rename
	{
		private readonly TieredFloatMenuOption option;
		public override void SetInitialSizeAndPosition()
		{
			windowRect = new Rect(originRect.x, originRect.y, InitialSize.x, InitialSize.y);
			windowRect = windowRect.Rounded();
		}
		public Dialog_RenameColonistGroup(TieredFloatMenu parentWindow, ColonistGroup colonistGroup, Rect originRect, Texture2D backgroundTexture, TieredFloatMenuOption option, string confirmationText)
			: base(parentWindow, colonistGroup, originRect, backgroundTexture, confirmationText)
		{
			this.originRect = new Rect(originRect.x + originRect.width, originRect.y, originRect.width, originRect.height);
			this.colonistGroup = colonistGroup;

			curName = colonistGroup.groupName;
			this.option = option;
		}

		protected override AcceptanceReport NameIsValid(string name)
		{
			return true;
		}

		protected override void SetName(string name)
		{
			colonistGroup.SetName(name);
			TacticDefOf.TG_RenameSFX.PlayOneShotOnCamera();
		}
	}
}
