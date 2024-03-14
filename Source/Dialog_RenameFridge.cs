using UnityEngine;
using Verse;


namespace RimFridge
{
	public class Dialog_RenameFridge : Dialog_RenameBuildingStorage
	{
		public Dialog_RenameFridge (IRenameable fridge) : base(fridge)
		{
			forcePause = true;
			doCloseX = true;
			closeOnClickedOutside = true;
			absorbInputAroundWindow = true;
			closeOnClickedOutside = true;
		}

		public override Vector2 InitialSize
		{
			get
			{
				var o = base.InitialSize;
				o.y += 50f;
				return o;
			}
		}

		protected override AcceptanceReport NameIsValid (string name)
		{
			if (name.Length == 0) return true;

			AcceptanceReport result = base.NameIsValid(name);
			return !result.Accepted ? result : AcceptanceReport.WasAccepted;
		}

		public override void DoWindowContents (Rect inRect)
		{
			base.DoWindowContents(inRect);

			if (
				Widgets.ButtonText(
					new Rect(15f, inRect.height - 35f - 15f - 50f, inRect.width - 15f - 15f, 35f),
					"ResetButton".Translate(),
					true,
					false,
					true
				)
			)
			{

				this.curName = null;
				this.OnRenamed(null);

				Find.WindowStack.TryRemove(this, true);
			}
		}

		protected override void OnRenamed (string name)
		{
			if ((name == null || name.Length == 0) && this.renaming != null)
			{
				this.renaming.RenamableLabel = null;
			}
		}
	}
}

