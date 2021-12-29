using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

		private Image bar; //Actual image of the bar, gathered using barTexture in ProgressBarRoutine()

		private Image progress; //Actual image of the progress, gathered using progressTexture in ProgressBarRoutine()

		private Image icon; //Actual image of the icon, gathered using iconTexture in ProgressBarRoutine()

		public string barTexture = "greenylie/inputprogressbar/bar";

		public string progressTexture = "greenylie/inputprogressbar/progress";

		public string iconTexture = "greenylie/inputprogressbar/icon_orangeberry";

		private RumbleStrength rumbleStrength; //RumbleStrength based on difficulty selected in the constructor

		public float inputMinusValue = 0.05f; //Subtraction number of the input progressbar (objective int 1)

		private int inputMinusValueTimerCurrent = 0; //Current value of MinusValue Timer refill

		public int inputMinusValueTimer = 15; //Maximum value of MinusValue Timer

		public float inputPlusValue = 0.1f; //Sum number of the input progressbar (objective int 1)

		private float inputCurrentValue = 0f; //Current value of the progressbar (requires 1 to succeed)

		public int failTimer = 5; //Time countdown for progressbar fail

		private bool wasPressed = false; //Used to verify unpress of the button to disable abusing

		private bool rotationNegative = false; //Variable to alternate rotation of the progressbar on input

		public InputProgressBar(string difficulty, float inputCurrentValue = 0, int failTimer = 5)
		{
			/*
			 * Generates an InputProgressBar entity which requires inputCurrentValue to reach 1 to succeed.
			 * Diffculties are predefined: super easy, easy, medium, hard, nightmare, impossible.
			 * You can specify the initial value of the progressbar by giving an inputCurrentValue
			 * inputPlusValue is the sum value per input and it's equal to 0.1
			 * inputMinusValue is the subtraction per timer
			 * inputMinusValueTimer is the timer for the subtraction, it changes with difficulties
			 * Textures are customizable by editing barTexture and progressTexture fields with a string of the GUI directoy
			 */

			base.Tag = Tags.HUD;
			this.level = Engine.Scene as Level;

			this.failTimer = failTimer;

			if (inputCurrentValue >= 0f && inputCurrentValue <= 0.9f) //Verifies that CurrentValue is a functional number, otherwise default 0
			{
				this.inputCurrentValue = inputCurrentValue;
			}
			else
			{
				this.inputCurrentValue = 0f;
			}

			if (difficulty.ToLower() == "super easy")
			{
				this.inputMinusValue = 0f;
				this.inputMinusValueTimerCurrent = 0;
			}
			else if (difficulty.ToLower() == "easy")
			{
				rumbleStrength = RumbleStrength.Climb;
				this.inputMinusValueTimer = 10;
			}
			else if (difficulty.ToLower() == "medium")
			{
				rumbleStrength = RumbleStrength.Light;
				this.inputMinusValueTimer = 5;
			}
			else if (difficulty.ToLower() == "hard")
			{
				rumbleStrength = RumbleStrength.Medium;
				this.inputMinusValueTimer = 3;
			}
			else if (difficulty.ToLower() == "nightmare")
			{
				rumbleStrength = RumbleStrength.Strong;
				this.inputMinusValueTimer = 2;
			}
			else if (difficulty.ToLower() == "impossible")
			{
				rumbleStrength = RumbleStrength.Strong;
				this.inputMinusValue = 0.15f;
				this.inputMinusValueTimer = 2;
			}
		}

		public IEnumerator MainRoutine()
		{
			yield return 0.5f;
			yield return ProgressBarRoutine();
			yield return ProgressBarIconRoutine();

			Stopwatch time = new Stopwatch(); //Used to verify elapsed seconds
			time.Start();

			while (inputCurrentValue < 1.0f && time.Elapsed.Seconds < failTimer)
			{
				Logger.Log("GreenylieHelper: InputProgressBar.MainRoutine - time.ElaspedGameTime.Seconds", time.Elapsed.Seconds.ToString());

				if (progress.Rotation != 0 && bar.Rotation != 0)
                {
					yield return ProgressBarChangeRotation(0f);
				}

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
					inputMinusValueTimerCurrent = inputMinusValueTimer;

					if (this.inputMinusValue != 0 && this.inputMinusValueTimer != 0 && inputCurrentValue != 0)
					{
						Input.Rumble(rumbleStrength, RumbleLength.Short);
					}

				}
				inputMinusValueTimerCurrent--;

				if (Input.MenuConfirm.Check && !wasPressed)
				{
					if (inputCurrentValue + inputPlusValue > 1.0f)
					{
						inputCurrentValue = 1.0f;
					}
					else
					{
						inputCurrentValue = inputCurrentValue + inputPlusValue;
					}

					yield return ProgressBarChangeRotation(auto: true);
					wasPressed = true;
				}
				else if (!Input.MenuConfirm.Check && wasPressed)
				{
					wasPressed = false;
				}

				if (progress.Position.X != inputCurrentValue)
				{
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
			progress.SetColor(Color.Orange);
			progress.CenterOrigin();
			icon = new Image(GFX.Gui[iconTexture]);
			icon.CenterOrigin();
			float percent = 0f;
			while (percent < 1f)
			{
				percent += Engine.DeltaTime;
				bar.Position = Vector2.Lerp(new Vector2(992f, 1080f + bar.Height / 2f), new Vector2(960f, 840f), Ease.BackOut(percent));
				bar.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
				progress.Position = Vector2.Lerp(new Vector2(992f, 1080f + progress.Height / 2f), new Vector2(960f, 840f), Ease.BackOut(percent));
				progress.Rotation = MathHelper.Lerp(0.5f, 0f, Ease.BackOut(percent));
				progress.Scale = new Vector2(inputCurrentValue, 1f);
				icon.Position = Vector2.Lerp(new Vector2(992f, 1080f + bar.Height / 2f), new Vector2(960f, 840f), Ease.BackOut(percent));
				yield return null;
			}
		}

		public IEnumerator ProgressBarIconRoutine()
        {
			
			float percent = 0f;
			Random rnd = new Random();
			while (percent < 1f)
			{
				percent += Engine.DeltaTime * 1.6f;
				icon.Rotation = MathHelper.Lerp(rnd.Next((new List<float> {2f,-2f}).Count), 0f, Ease.BackInOut(percent));
				yield return null;
			}
		}

		public IEnumerator ProgressBarChangeXValue(float value)
		{
			progress.Scale = Vector2.SmoothStep(progress.Scale, new Vector2(value, progress.Scale.Y), 0.5f);
			yield return null;
		}

		public IEnumerator ProgressBarChangeRotation(float value = 0f, bool auto = false)
		{
            if (auto)
            {
				value = 0.05f;
				if (this.rotationNegative) value = value * -1; else rotationNegative = false;
            }
			bar.Rotation = MathHelper.SmoothStep(bar.Rotation, value, 0.3f);
			progress.Rotation = MathHelper.SmoothStep(progress.Rotation, value, 0.3f);
			icon.Rotation = MathHelper.SmoothStep(icon.Rotation, (value * -1.5f) , 0.3f);
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
			if (progress != null && progress.Visible)
			{
				progress.Update();
			}
			if (bar != null && progress.Visible)
            {
				bar.Update();
            }
			if (icon != null && icon.Visible)
			{
				icon.Update();
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
			if (icon != null && icon.Visible)
			{
				icon.Render();
			}
		}
	}
}
