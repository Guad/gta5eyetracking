using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;

namespace Gta5EyeTracking
{
	public class Aiming
	{
		public bool AlwaysShowCrosshair { get; set; }

		private readonly Stopwatch _shootStopWatch;
		private UIContainer _uiContainerCrosshair;
		
		private bool _drawCrosshair;
		private Vector2 _crosshairPosition;
		private readonly Settings _settings;

		public Aiming(Settings settings)
		{
			_settings = settings;
			_shootStopWatch = new Stopwatch();
			_shootStopWatch.Restart();
			CreateCrosshair();
		}

		private void CreateCrosshair()
		{
			_uiContainerCrosshair = new UIContainer(new Point(0, 0), new Size(4, 4), Color.FromArgb(0, 0, 0, 0));
			var crosshair1 = new UIRectangle(new Point(0, 0), new Size(4, 4), Color.FromArgb(220, 0, 0, 0));
			_uiContainerCrosshair.Items.Add(crosshair1);
			var crosshair2 = new UIRectangle(new Point(1, 1), new Size(2, 2), Color.FromArgb(220, 255, 255, 255));
			_uiContainerCrosshair.Items.Add(crosshair2);
		}

		public void Shoot(Vector3 target)
		{
			_drawCrosshair = true;
			var weaponPos = Game.Player.Character.Position;

			//take velocity into account
			if (Game.Player.Character.IsInVehicle())
			{
				var vehicle = Game.Player.Character.CurrentVehicle;
				weaponPos += vehicle.Velocity * 0.06f;
			}

			var fireRateTime = TimeSpan.FromSeconds(0.2);
			if (_shootStopWatch.Elapsed > fireRateTime)
			{
				//Util.PlayAnimation(Game.Player.Character, "weapons@rifle@lo@smg", "fire_med", 8.0f, -1, false, 0);
				//Game.Player.Character.Task.ClearAll();
				//Game.Player.Character.Task.ShootAt(target, (int)fireRateTime.TotalMilliseconds + 50);
				World.ShootBullet(weaponPos, target, Game.Player.Character, new Model(Game.Player.Character.Weapons.Current.Hash), 1);
				_shootStopWatch.Restart();
			}
		}

		public void Tase(Vector3 target)
		{
			_drawCrosshair = true;
			var weaponPos = Game.Player.Character.Position;

			//take velocity into account
			if (Game.Player.Character.IsInVehicle())
			{
				var vehicle = Game.Player.Character.CurrentVehicle;
				weaponPos += vehicle.Velocity * 0.06f;
			}

			var directionVector = (target - weaponPos);
			directionVector.Normalize();
			var shockPos = target - directionVector;

			var fireRateTime = TimeSpan.FromSeconds(0.2);
			if (_shootStopWatch.Elapsed > fireRateTime)
			{
				//Util.PlayAnimation(Game.Player.Character, "weapons@rifle@lo@smg", "fire_med", 8.0f, -1, false, 0);

				World.ShootBullet(shockPos, target, Game.Player.Character, WeaponHash.StunGun, 1);
				_shootStopWatch.Restart();
			}
		}

		public void ShootMissile(Vector3 target)
		{
			var weaponPos = Game.Player.Character.Position;

			//take velocity into account
			if (Game.Player.Character.IsInVehicle())
			{
				var vehicle = Game.Player.Character.CurrentVehicle;
				weaponPos += vehicle.Velocity * 0.06f;
			}

			var fireRateTime = TimeSpan.FromSeconds(0.2);
			if (_shootStopWatch.Elapsed > fireRateTime)
			{
				World.ShootBullet(weaponPos, target, Game.Player.Character, WeaponHash.HomingLauncher, 1);
				_shootStopWatch.Restart();
			}
		}

		public void Incinerate(Vector3 target)
		{
			//var dist = (target - Game.Player.Character.Position).Length();
			//if (dist > 3)
			{
				World.AddExplosion(target, ExplosionType.Molotov1, 2, 0);
			}
		}

		public void Water(Vector3 target)
		{
			_drawCrosshair = true;
			var dist = (target - Game.Player.Character.Position).Length();
			if (dist > 3)
			{
				World.AddExplosion(target, ExplosionType.ValveWater1, 2, 0);
			}
		}

		public void Process()
		{
			if (_settings.AimWithGazeEnabled 
				&& (GameplayCamera.IsAimCamActive
					|| (!Game.Player.Character.IsInVehicle()
						&& Game.IsKeyPressed(Keys.B)))
					|| (Game.Player.Character.IsInPlane || Game.Player.Character.IsInHeli))
			{
				_drawCrosshair = true;
			}

			if (_drawCrosshair || AlwaysShowCrosshair)
			{
				_uiContainerCrosshair.Draw();
			}


			_drawCrosshair = false;
		}

		public void MoveCrosshair(Vector2 screenCoords)
		{
			var uiWidth = UI.WIDTH;
			var uiHeight = UI.HEIGHT;

			var crosshairPosition = new Vector2(uiWidth * 0.5f + screenCoords.X * uiWidth * 0.5f - 2, uiHeight * 0.5f + screenCoords.Y * uiHeight * 0.5f - 2);
			const float w = 1;//Filtering is done earlier 0.6f;
			_crosshairPosition = new Vector2(_crosshairPosition.X + (crosshairPosition.X - _crosshairPosition.X) * w,
				_crosshairPosition.Y + (crosshairPosition.Y - _crosshairPosition.Y) * w);

			_uiContainerCrosshair.Position = new Point((int)_crosshairPosition.X, (int)_crosshairPosition.Y);
		}
	}
}