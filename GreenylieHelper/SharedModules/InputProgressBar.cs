using System;
using System.Collections;
using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.GreenylieHelper.SharedModules
{
	[Tracked(true)]
	[CustomEntity(new string[] { "GreenylieHelper/InputProgressBar" })]
	public class InputProgressBar : Entity
	{
		private Level level;

		private Image bar;

		private Image progress;

		private bool waitForKeyPress;

		private bool wasPressed = false;

		private string barTexture;

		private string progressTexture;

		private float timer;

		private float percent = 0f;

		public float inputMinusValue = 0.05f;

		private int inputMinusValueTimerCurrent = 0;

		private int inputMinusValueTimer = 15;

		public float inputPlusValue = 0.1f;

		private float inputCurrentValue = 0f;


		public InputProgressBar(float inputPlusValue, float inputCurrentValue, string difficulty, string barTexture, string progressTexture)
		{
			base.Tag = Tags.HUD;
			this.level = Engine.Scene as Level;
			if (inputPlusValue > 0f && inputPlusValue <= 1f) this.inputPlusValue = inputPlusValue;
			if (inputCurrentValue >= 0f && inputCurrentValue <= 0.9f) this.inputCurrentValue = inputCurrentValue;

			if (difficulty.ToLower() == "super easy")
            {
				this.inputMinusValue = 0f;
            }
			else if (difficulty.ToLower() == "easy")
            {
				this.inputMinusValueTimer = 20;
            }
			else if (difficulty.ToLower() == "medium")
            {
				this.inputMinusValueTimer = 15;
            }
			else if (difficulty.ToLower() == "hard")
            {
				this.inputMinusValueTimer = 10;
            }
			else if (difficulty.ToLower() == "nightmare")
            {
				this.inputMinusValueTimer = 5;
            }
			else if (difficulty.ToLower() == "impossible")
            {
				this.inputMinusValueTimer = 1;
            }

			this.barTexture = barTexture;
			this.progressTexture = progressTexture;
		}

		public IEnumerator MainRoutine()
		{
			yield return 0.5f;
			yield return ProgressBarRoutine();

			while (inputCurrentValue < 1.0f)
            {

				if (!Input.MenuConfirm.Pressed)
                {
					if (inputMinusValueTimerCurrent == 0)
                    {
						if (inputCurrentValue - inputMinusValue < 0f)
						{
							inputCurrentValue = 0f;
						}
						else
						{
							inputCurrentValue = inputCurrentValue - inputMinusValue;
						}
						yield return ProgressBarChangeXValue(inputCurrentValue);
						inputMinusValueTimerCurrent = inputMinusValueTimer;
					}
					inputMinusValueTimerCurrent--;
					yield return null;
                }
				else
                {
					if (inputCurrentValue + inputPlusValue > 1.0f)
					{
						inputCurrentValue = 1.0f;
					}
					else
					{ 
						inputCurrentValue = inputCurrentValue + inputPlusValue;
					}
					Logger.Log("GreenylieHelper", "Press");
					yield return ProgressBarChangeXValue(inputCurrentValue);
				}

            }
			yield return EndRoutine();
		}

		public IEnumerator ProgressBarRoutine()
		{
			bar = new Image(GFX.Gui[barTexture]);
			bar.CenterOrigin();
			progress = new Image(GFX.Gui[progressTexture]);
			progress.CenterOrigin();
			percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime;
				bar.Position = Vector2.Lerp(new Vector2(992f, 1080f + bar.Height / 2f), new Vector2(960f, 540f), Ease.BackOut(percent));
				bar.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
				progress.Position = Vector2.Lerp(new Vector2(992f, 1080f + progress.Height / 2f), new Vector2(960f, 540f), Ease.BackOut(percent));
				progress.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
				progress.Scale = new Vector2(0f,1f);
				yield return null;
			}
		}

		public IEnumerator ProgressBarChangeXValue(float value)
        {
			Logger.Log("GreenylieHelper: InputProgressBar.inputCurrentValue", inputCurrentValue.ToString());
			Logger.Log("GreenylieHelper: InputProgressBar.ProgressBarChangeXValue", "X changed from " + progress.Scale.X.ToString() + " to " + value.ToString());
			percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime;
				progress.Scale = Vector2.Lerp(progress.Scale, new Vector2(value, progress.Scale.Y), Ease.BackIn(percent));
			}
			yield return null;
        }

		public IEnumerator EndRoutine()
		{
			float percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime * 2f;
				bar.Position = Vector2.Lerp(bar.Position, new Vector2(928f, (0f - bar.Height) / 2f), Ease.BackIn(percent));
				bar.Rotation = MathHelper.Lerp(bar.Rotation, -0.15f, Ease.BackIn(percent));
				progress.Position = Vector2.Lerp(progress.Position, new Vector2(928f, (0f - bar.Height) / 2f), Ease.BackIn(percent));
				progress.Rotation = MathHelper.Lerp(progress.Rotation, -0.15f, Ease.BackIn(percent));
				yield return null;
			}
			yield return null;
			level.Remove(this);
		}

		public override void Update()
		{
			if (waitForKeyPress)
			{
				timer += Engine.DeltaTime;
			}
			if (progress != null && progress.Visible)
			{
				progress.Update();
			}
		}

		public override void Render()
		{
			Level level = base.Scene as Level;
			if (level != null && (level.FrozenOrPaused || level.RetryPlayerCorpse != null || level.SkippingCutscene))
			{
				return;
			}
			if (bar != null && bar.Visible)
			{
				bar.Render();
			}
			if (progress != null && progress.Visible)
			{
				progress.Render();
			}
		}
	}
}
