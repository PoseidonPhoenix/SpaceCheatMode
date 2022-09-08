using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
using System.Windows.Media.Animation;
using Newtonsoft.Json;

namespace SpaceCheatMode
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>


	public enum DllInjectionResult
	{
		DllNotFound,
		GameProcessNotFound,
		InjectionFailed,
		Success
	}

	public partial class MainWindow : Window
	{
		Memory.Mem memory = new Memory.Mem();

		readonly byte[] RevealImpBytes = { 0x90, 0x90, 0x90, 0x90 };
		readonly byte[] InvisibilityBytes = { 0x90, 0x90, 0x90, 0x90 };
		readonly byte[] InvisibilityResetBytes = { 0xC6, 0x40, 0x30, 0x00 };
		readonly byte[] StopRevealImpBytes = { 0x80, 0x78, 0x2C, 0x00 };
		readonly byte[] KillCooldownBypass = { 0xF3, 0x0F, 0x11, 0x45, 0x0C };
		readonly byte[] KillCooldownReset = { 0x0F, 0x83, 0xA3, 0x00, 0x00, 0x00 };
		readonly byte[] LockOnTarget = { 0xE9, 0x79, 0x02, 0x00, 0x00, 0x90 };
		readonly byte[] CancelLockingOnTarget = { 0x0F, 0x84, 0x78, 0x02, 0x00, 0x00 };
		readonly byte[] KillOtherImpostor = { 0x0F, 0x82, 0x59, 0x01, 0x00, 0x00 };
		readonly byte[] KillOtherImpostorReset = { 0x0F, 0x85, 0x59, 0x01, 0x00, 0x00 };
		readonly byte[] InfiniteKillRangeBytes = { 0xC7, 0x44, 0x06, 0x10, 0x00, 0x00, 0x80, 0x7F, 0xF3, 0x0F, 0x10, 0x44, 0x06, 0x10 };
		readonly byte[] InfiniteKillRangeReset = { 0xF3, 0x0F, 0x10, 0x44, 0x86, 0x10 };
		readonly byte[] KillThroughWalls = { 0x72, 0x23 };
		readonly byte[] KillThroughWallsReset = { 0x75, 0x23 };
		bool UnlimitedEMeetings = false;
		bool InsRepDis = false;
		bool NoClip = false;
		bool KillCooldownBypassed = false;
		bool LockedOnTarget = false;
		bool RevealedImp = false;
		bool EmMeetCooldownBypassed = false;
		bool InvisibilityEnabled = false;
		bool InfiniteVisionEnabled = false;
		bool KillOtherImpostorEnabled = false;
		bool InfiniteKillRangeEnabled = false;
		bool FlippedMapEnabled = false;
		bool UnlockedVents = false;
		bool TasksAsImpEnabled = false;
		bool TimeBanRemoved = false;
		bool SeeGhostStuffActivated = false;
		bool InfiniteSabotageEnabled = false;
		bool ForceThreadClose = false;
		bool SateliteViewEnabled = false;
		bool CheatsEnabled = false;
		bool AllSameColorEnabled = false;
		bool RainbowLPlayer = false;
		bool DisableReportAndMeetings = false;
		bool RainbowNameInitiated = false;
		bool HostPageArrowFlipped = false;
		bool LockCameraEnabled = false;
		bool AllGhostsEnabled = false;
		bool AllPlayersRainbow = false;
		bool DisableVoting = false;
		bool AllGhostsServerSidedEnabled = false;
		bool IsJsonCreated = false;
		bool FakeCamerasEnabled = false;
		//bool SkinsUnlocked = false;
		bool EnableSabotage = false;
		bool isChatUnlocked = false;
		bool UIChangesEnabled = false;

		dynamic jsonfile;

		double AmongUsScalingUIAddressesValue;
		double mvar;

		string selecteddll;
		
		readonly public string Infrange1 = "GameAssembly.dll+53494A";

		//string p1x = "GameAssembly.dll+01C879B8,5C,00,60,30,8,5C,2C";
		//string p1y = "GameAssembly.dll+01C879B8,5C,00,60,30,8,5C,30";

		private readonly string[] NamePointers = 
		{ 
			"GameAssembly.dll+01C879B8,5C,8,8,10,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,14,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,18,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,1C,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,20,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,24,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,28,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,2C,34,C,C", 
			"GameAssembly.dll+01C879B8,5C,8,8,30,34,C,C",
			"GameAssembly.dll+01C879B8,5C,8,8,34,34,C,C"
		};

		private readonly string[] ColorIDSPointers = 
		{ 
			"GameAssembly.dll+01C879B8,5C,8,8,10,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,14,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,18,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,1C,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,20,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,24,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,28,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,2C,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,30,34,14",
			"GameAssembly.dll+01C879B8,5C,8,8,34,34,14"
		};

		private readonly string[] RoleAddressPointers = 
		{ 
			"GameAssembly.dll+01C879B8,5C,8,8,10,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,14,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,18,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,1C,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,20,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,24,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,28,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,2C,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,30,34,2C",
			"GameAssembly.dll+01C879B8,5C,8,8,34,34,2C"
		};

		Storyboard sb;
		Storyboard OpenStoryboard;
		Storyboard DisableStckP;
		Storyboard RotateH_PageArrow;
		Storyboard ResetH_PageArrowRotation;
		Storyboard MoveHostpage1;
		Storyboard ResetHostPage1;

		//Color UIColor1;


		#region Injector
		static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int CloseHandle(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

		[DllImport("kernel32.dll", SetLastError = true)]
		static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
			IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);


		bool bInject(uint pToBeInjected, string sDllPath)
		{
			IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

			if (hndProc == INTPTR_ZERO)
			{
				return false;
			}

			IntPtr lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

			if (lpLLAddress == INTPTR_ZERO)
			{
				return false;
			}

			IntPtr lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0x40);

			if (lpAddress == INTPTR_ZERO)
			{
				return false;
			}

			byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

			if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)
			{
				return false;
			}

			if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null) == INTPTR_ZERO)
			{
				return false;
			}

			CloseHandle(hndProc);

			return true;
		} 
		#endregion

		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}


		private void Button_Click_1(object sender, RoutedEventArgs e)
		{

			memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,14", "float", SpeedValueChanger.Text);

		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{

		}

		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
			sb = Application.Current.MainWindow.FindResource("epicanimation") as Storyboard;
			sb.Begin();
			try
			{
				string MagicMushroomDllFullPath = Path.GetFullPath("MagicMushroom.dll");
				try
				{
					bInject((uint)Process.GetProcessesByName("Among Us").FirstOrDefault().Id, MagicMushroomDllFullPath);
					IsJsonCreated = true;
				}
				catch
				{
					MessageBox.Show("Cannot find MagicMushroom.dll or Among Us is not open.", "Error");
				}
				memory.OpenProcess(Process.GetProcessesByName("Among Us").FirstOrDefault().Id);
				CheatsEnabled = true;
			}
			catch (Exception)
			{
				MessageBox.Show("Oops Something Went Wrong. Please Open Among Us v2022.7.12e (Epic Games) Then Retry!", "Error");
				EnableCheats.IsChecked = false;
			}

		}


		private void CrewEnable_Checked(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,34,2C", "int", "0");
			if (ImpEnable.IsChecked == true)
			{
				ImpEnable.IsChecked = false;
			}
			if (CrewGhostEnable.IsChecked == true)
			{
				CrewGhostEnable.IsChecked = false;
			}
			if (ImpGhostEnable.IsChecked == true)
			{
				ImpGhostEnable.IsChecked = false;
			}
		}

		private void ImpEnable_Checked(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,34,2C", "int", "1");
			if (CrewEnable.IsChecked == true)
			{
				CrewEnable.IsChecked = false;
			}
			if (CrewGhostEnable.IsChecked == true)
			{
				CrewGhostEnable.IsChecked = false;
			}
			if (ImpGhostEnable.IsChecked == true)
			{
				ImpGhostEnable.IsChecked = false;
			}
		}

		private void CrewGhostEnable_Checked(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,34,2C", "int", "256");
			if (ImpEnable.IsChecked == true)
			{
				ImpEnable.IsChecked = false;
			}
			if (CrewEnable.IsChecked == true)
			{
				CrewEnable.IsChecked = false;
			}
			if (ImpGhostEnable.IsChecked == true)
			{
				ImpGhostEnable.IsChecked = false;
			}
		}

		private void ImpGhostEnable_Checked(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,34,2C", "int", "257");
			if (ImpEnable.IsChecked == true)
			{
				ImpEnable.IsChecked = false;
			}
			if (CrewGhostEnable.IsChecked == true)
			{
				CrewGhostEnable.IsChecked = false;
			}
			if (CrewEnable.IsChecked == true)
			{
				CrewEnable.IsChecked = false;
			}
		}

		private void Vision_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			int Roleval = memory.ReadInt("GameAssembly.dll+01C879B8,5C,0,34,2C");
			switch (Roleval)
			{
				case 0:
					memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,18", "float", Vision.Value.ToString());
					break;
				case 1:
					memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,1C", "float", Vision.Value.ToString());
					break;
			}
		}

		private void UnlimitedEmeetingsSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!UnlimitedEMeetings)
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,48", "int", "9999");
				UnlimitedEMeetings = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,48", "int", "1");
				UnlimitedEMeetings = false;
			}
		}

		private void InsRepDisSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!InsRepDis)
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,2C", "float", "15");
				InsRepDis = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,2C", "float", "2");
				InsRepDis = false;
			}
		}

		private void NoClipSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

			if (!NoClip)
			{
				memory.WriteMemory("UnityPlayer.dll+9B97B7", "bytes", "0F 85");
				NoClip = true;
			}
			else
			{
				memory.WriteMemory("UnityPlayer.dll+9B97B7", "bytes", "0F 84");
				NoClip = false;
			}

		}


		private void KillCDBypassSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

			if (!KillCooldownBypassed)
			{
				memory.WriteBytes("GameAssembly.dll+65C3F3", KillCooldownBypass);
				KillCooldownBypassed = true;


			}
			else
			{
				memory.WriteBytes("GameAssembly.dll+65C3F3", KillCooldownReset);
				KillCooldownBypassed = false;
			}


		}

		

		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			ForceThreadClose = true;
			Process.GetCurrentProcess().Kill();
		}

		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			WindowState = WindowState.Minimized;
		}
		

		private void LockOnTargetSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!LockedOnTarget)
			{
				memory.WriteBytes("GameAssembly.dll+6579E2", LockOnTarget);
				LockedOnTarget = true;
			}
			else
			{
				memory.WriteBytes("GameAssembly.dll+6579E2", CancelLockingOnTarget);
				LockedOnTarget = false;
			}
		}
		
		private void RevealImpostorsSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!RevealedImp)
			{
				//memory.WriteBytes("GameAssembly.dll+17CAAEA", RevealImpBytes);
				memory.WriteBytes("GameAssembly.dll+6C20A4", RevealImpBytes);
				RevealedImp = true;
			}
			else
			{
				//memory.WriteMemory("GameAssembly.dll+17CAAEA", "bytes", "80 7B 2C 00");
				memory.WriteBytes("GameAssembly.dll+6C20A4", StopRevealImpBytes);
				RevealedImp = false;
			}
		}

		private void EmergencyMeetingsCooldownBypassSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!EmMeetCooldownBypassed)
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,34", "int", "0");
				EmMeetCooldownBypassed = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,34", "int", "10");
				EmMeetCooldownBypassed = false;
			}
		}

		private void InvisibilitySwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!InvisibilityEnabled)
			{
				memory.WriteBytes("GameAssembly.dll+81279D", InvisibilityBytes);
				InvisibilityEnabled = true;
			}
			else
			{
				memory.WriteBytes("GameAssembly.dll+81279D", InvisibilityResetBytes);
				InvisibilityEnabled = false;
			}
		}
		

		private void ShowOpenFileDialog()
		{
			OpenFileDialog showOpenFileDialogInitiate = new OpenFileDialog();
			showOpenFileDialogInitiate.InitialDirectory = @"C:\";
			showOpenFileDialogInitiate.Title = "Select A Dll";
			showOpenFileDialogInitiate.DefaultExt = "dll";
			showOpenFileDialogInitiate.Filter = "dll files (*.dll)|*.dll";
			showOpenFileDialogInitiate.FilterIndex = 2;

			if (showOpenFileDialogInitiate.ShowDialog() == true)
			{
				if (showOpenFileDialogInitiate.FileName != "")
				{
					selecteddll = showOpenFileDialogInitiate.FileName;
				}
			}
		}

		private void DllInjectorButton_Click(object sender, RoutedEventArgs e)
		{
			ShowOpenFileDialog();
			try
			{
				bInject((uint)Process.GetProcessesByName("PSUAConsole").FirstOrDefault().Id, selecteddll);
			}
			catch (Exception)
			{
				MessageBox.Show("Something Went Wrong\nMake Sure That You Selected A Valid File And That You Have Among Us Open", "Error");
			}

		}
		

		private void InfiniteVisionSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			/*
			int Rolevalue = memory.ReadInt("GameAssembly.dll+01C879B8,5C,0,34,28");
			switch (Rolevalue)
			{
				case 0:
					memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,18", "float", "10000");
					break;
				case 1:
					memory.WriteMemory("GameAssembly.dll+01C879B8,5C,4,1C", "float", "10000");
					break;
			}
			*/
			if (!InfiniteVisionEnabled)
			{
				//memory.WriteMemory("UnityPlayer.dll+145A24C,C,34,20,10,20", "float", "0.0");
				memory.WriteMemory("UnityPlayer.dll+995363", "bytes", "8B 41 2C");
				InfiniteVisionEnabled = true;
			}
			else
			{
				//memory.WriteMemory("UnityPlayer.dll+145A24C,C,34,20,10,20", "float", "1.0");
				memory.WriteMemory("UnityPlayer.dll+995363", "bytes", "8B 49 2C");
				InfiniteVisionEnabled = false;
			}
		}

		private void PlayWeaponsAnimationButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["playweaponsanimationsignal"]).ToString("X"), "byte", "1");
		}

		private void PlayTrashAnimationButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["playtrashanimationsignal"]).ToString("X"), "byte", "1");
		}

		private void DoScanAnimationButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ActivateScanAddress"]).ToString("X"), "byte", "1");
		}

		private void KillOtherImpostorSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!KillOtherImpostorEnabled)
			{
				memory.WriteBytes("GameAssembly.dll+534A3E", KillOtherImpostor);
				KillOtherImpostorEnabled = true;
			}
			else
			{
				memory.WriteBytes("GameAssembly.dll+534A3E", KillOtherImpostorReset);
				KillOtherImpostorEnabled = false;
			}
		}
		

		private void InfiniteKillRangeSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!InfiniteKillRangeEnabled)
			{
				memory.CreateCodeCave(Infrange1, InfiniteKillRangeBytes, 6);
				memory.WriteBytes("GameAssembly.dll+534B72", KillThroughWalls);
				InfiniteKillRangeEnabled = true;
			}
			else
			{
				memory.WriteBytes(Infrange1, InfiniteKillRangeReset);
				memory.WriteBytes("GameAssembly.dll+534B72", KillThroughWallsReset);
				InfiniteKillRangeEnabled = false;
			}
		}
		
		private void PlayShieldsAnimationButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["playshieldsanimationsignal"]).ToString("X"), "byte", "1");
		}

		private void FlipMapSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!FlippedMapEnabled)
			{
				memory.WriteMemory("GameAssembly.dll+69FC80", "bytes", "B0 01");
				memory.WriteMemory("GameAssembly.dll+69FC82", "bytes", "C3");
				FlippedMapEnabled = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+69FC80", "bytes", "55");
				memory.WriteMemory("GameAssembly.dll+69FC81", "bytes", "8B EC");
				FlippedMapEnabled = false;
			}
		}


		private void VentAsCrewmateSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!UnlockedVents)
			{
				memory.WriteMemory("GameAssembly.dll+800344", "bytes", "90 90 90 90");
				UnlockedVents = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+800344", "bytes", "80 7F 2C 00");
				UnlockedVents = false;
			}
		}


		private void LocalPlayerChangeHatButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["HatSelected"]).ToString("X"), "int", HatIDTextBox.Text);
			memory.WriteMemory((jsonfile["changehatinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerChangePetButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["PetSelected"]).ToString("X"), "int", PetIDTextBox.Text);
			memory.WriteMemory((jsonfile["changepetinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerChangeSkinButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["SkinSelected"]).ToString("X"), "int", SkinIDTextBox.Text);
			memory.WriteMemory((jsonfile["changeskininitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerBlueButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "1");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");

		}

		private void LocalPlayerGreenButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "2");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerYellowButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "5");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerRedButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "0");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerBrownButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "9");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerBlackButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "6");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerPinkButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "3");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerOrangeButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "4");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerLimeButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "11");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerPurpleButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "8");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void LocalPlayerCyanButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "10");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void SNTest_Click(object sender, RoutedEventArgs e)
		{
			char[] splitted = NameInputTextBox.Text.ToCharArray();
			int sCounter = 0;
			if (splitted.Length > 10)
			{
				for (int U = splitted.Length - 1; U > 9; --U)
				{
					int indexToRemove = U;
					splitted = splitted.Where((source, index) => index != indexToRemove).ToArray();
				}
			}
			for (int B = 0; B < 21; B += 2)
			{
				string address_for_clean_up2 = (jsonfile["name_container2"] + B).ToString("X");
				memory.WriteMemory(address_for_clean_up2, "byte", "00");
			}
			foreach (char ch in splitted)
			{
				string charactbyte = ((byte)ch).ToString("X");
				string full_address_of_name_container2 = (jsonfile["name_container2"] + sCounter).ToString("X");
				memory.WriteMemory(full_address_of_name_container2, "byte", charactbyte);
				sCounter += 2;
			}
			memory.WriteMemory((jsonfile["changenameinitiated"]).ToString("X"), "byte", "1");

		}


		private void OnLoad(object sender, RoutedEventArgs e)
		{

		}

		private void EnableTasksAsImpostorSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!TasksAsImpEnabled)
			{
				memory.WriteMemory("GameAssembly.dll+69F410", "bytes", "90 90 90 90");
				TasksAsImpEnabled = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+69F410", "bytes", "80 78 2C 00");
				TasksAsImpEnabled = false;
			}
		}

		private void RemoveTimeBanSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!TimeBanRemoved)
			{
				memory.WriteMemory("GameAssembly.dll+7A0870", "bytes", "B8 00 00 00 00 C3");
				TimeBanRemoved = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+7A0870", "bytes", "55 8B EC 6A 00 FF");
				TimeBanRemoved = false;
			}
		}

		private void SeeGhostStuffSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!SeeGhostStuffActivated)
			{
				//memory.WriteMemory("GameAssembly.dll+673AA2", "bytes", "80 78 2D 01");
				memory.WriteMemory("GameAssembly.dll+534E59", "bytes", "0F 83");
				SeeGhostStuffActivated = true;
			}
			else
			{
				//memory.WriteMemory("GameAssembly.dll+673AA2", "bytes", "80 78 2D 00");
				memory.WriteMemory("GameAssembly.dll+534E59", "bytes", "0F 84");
				SeeGhostStuffActivated = false;
			}
		}

		private void AllPlayerBlueButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "1");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerGreenButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "2");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerYellowButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "5");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerRedButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "0");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerBrownButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "9");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void StatsCheckerTask()
		{
			if (!ForceThreadClose)
			{
				Label[] PlayerNames = { Player1Name, Player2Name, Player3Name, Player4Name, Player5Name, Player6Name, Player7Name, Player8Name, Player9Name, Player10Name };
				System.Windows.Shapes.Rectangle[] PlayerRects = { Player1Rect, Player2Rect, Player3Rect, Player4Rect, Player5Rect, Player6Rect, Player7Rect, Player8Rect, Player9Rect, Player10Rect };
				for (int j = 0; j < 10; ++j)
				{
					byte[] UnicodeNameInBytes = memory.ReadBytes(NamePointers[j], 20);
					if (UnicodeNameInBytes == null)
					{
						PlayerNames[j].Content = "?????????";
						PlayerNames[j].Foreground = Brushes.White;
						continue;
					}
					string UnicodeName = Encoding.Unicode.GetString(UnicodeNameInBytes);
					var endOfStringPosition = UnicodeName.IndexOf("\0");
					string FinalName = endOfStringPosition == -1 ? UnicodeName : UnicodeName.Substring(0, endOfStringPosition);
					PlayerNames[j].Content = FinalName;
				}
				for (int b = 0; b < 10; ++b)
				{
					byte[] UnicodeNameInBytesForColor = memory.ReadBytes(NamePointers[b], 20);
					switch (memory.ReadByte(ColorIDSPointers[b]))
					{
						case 0:
							if (UnicodeNameInBytesForColor != null)
							{
								PlayerNames[b].Foreground = Brushes.Red;
							}
							break;
						case 1:
							PlayerNames[b].Foreground = Brushes.Blue;
							break;
						case 2:
							PlayerNames[b].Foreground = Brushes.Green;
							break;
						case 3:
							PlayerNames[b].Foreground = Brushes.Pink;
							break;
						case 4:
							PlayerNames[b].Foreground = Brushes.Orange;
							break;
						case 5:
							PlayerNames[b].Foreground = Brushes.Yellow;
							break;
						case 6:
							PlayerNames[b].Foreground = Brushes.Black;
							break;
						case 7:
							PlayerNames[b].Foreground = Brushes.White;
							break;
						case 8:
							PlayerNames[b].Foreground = Brushes.Purple;
							break;
						case 9:
							PlayerNames[b].Foreground = Brushes.Brown;
							break;
						case 10:
							PlayerNames[b].Foreground = Brushes.Cyan;
							break;
						case 11:
							PlayerNames[b].Foreground = Brushes.Lime;
							break;


					}

					for (int xD = 0; xD < 10; ++xD)
					{
						if (memory.ReadInt(RoleAddressPointers[xD]) == 1)
						{
							PlayerRects[xD].Stroke = Brushes.Red;
						}
						else if (memory.ReadInt(RoleAddressPointers[xD]) == 0)
						{
							PlayerRects[xD].Stroke = Brushes.Aqua;
						}
						else
						{
							PlayerRects[xD].Stroke = Brushes.White;
						}
					}

				}
			}
		}

		private void AllPlayerBlackButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "6");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerOrangeButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "4");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerLimeButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "11");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerPinkButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "3");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerPurpleButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "8");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void AllPlayerCyanButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "10");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void InfiniteSabotageSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!InfiniteSabotageEnabled)
			{
				memory.WriteMemory("GameAssembly.dll+4B6A8E", "bytes", "C7 47 08 00 00 00 00");
				memory.WriteMemory("GameAssembly.dll+4B6AC8", "bytes", "C7 47 08 00 00 00 00");
				memory.WriteMemory("GameAssembly.dll+4B6B06", "bytes", "C7 47 08 00 00 00 00");
				memory.WriteMemory("GameAssembly.dll+4B6B40", "bytes", "C7 47 08 00 00 00 00");
				memory.WriteMemory("GameAssembly.dll+4B6BAC", "bytes", "C7 47 08 00 00 00 00");
				memory.WriteMemory("GameAssembly.dll+5AEF74", "bytes", "C7 47 38 00 00 00 00");
				InfiniteSabotageEnabled = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+4B6A8E", "bytes", "C7 47 08 00 00 F0 41");
				memory.WriteMemory("GameAssembly.dll+4B6AC8", "bytes", "C7 47 08 00 00 F0 41");
				memory.WriteMemory("GameAssembly.dll+4B6B06", "bytes", "C7 47 08 00 00 F0 41");
				memory.WriteMemory("GameAssembly.dll+4B6B40", "bytes", "C7 47 08 00 00 F0 41");
				memory.WriteMemory("GameAssembly.dll+4B6BAC", "bytes", "C7 47 08 00 00 F0 41");
				memory.WriteMemory("GameAssembly.dll+5AEF74", "bytes", "C7 40 38 00 00 F0 41");
				InfiniteSabotageEnabled = false;
			}
		}

		private void AdjustUIColors()
		{
			PrimaryUIHueGradient.Color = ColorPicker1.Color;
			SecondaryUIHueGradient.Color = ColorPicker2.Color;
		}

		private void Player5Name_Loaded(object sender, RoutedEventArgs e)
		{
			Thread StatsChecker = new Thread(StatsCheckerEpic);

			StatsChecker.Start();
		}

		private void StatsCheckerEpic()
		{
			Thread.Sleep(1000);
			while (true)
			{
				if (IsJsonCreated)
				{
					Thread.Sleep(7000);
					jsonfile = JsonConvert.DeserializeObject(File.ReadAllText("Addresses.json"));
					Thread.Sleep(1000);
					memory.WriteMemory((jsonfile["name_container"]).ToString("X"), "bytes", "4F 00 4F 00 4F 00 4F 00 4F 00 4F");
					memory.WriteMemory((jsonfile["name_container3"]).ToString("X"), "bytes", "4F 00 4F 00 4F 00 4F 00 4F 00 4F");
					IsJsonCreated = false;
				}
				if (CheatsEnabled)
				{
					Application.Current.Dispatcher.Invoke(StatsCheckerTask);
				}
				if (UIChangesEnabled)
				{
					Application.Current.Dispatcher.Invoke(AdjustUIColors);
				}
				Thread.Sleep(1500);
			}
		}

		private void SateliteViewSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!SateliteViewEnabled)
			{
				if (memory.ReadFloat("UnityPlayer.dll+0145A24C,C,18,44,7C") != 0)
				{
					memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,7C", "float", "0");
				}
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,78", "float", "0.5");
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,80", "float", "4");
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,84", "float", "4");

				SateliteViewEnabled = true;
			}
			else
			{
				if (memory.ReadFloat("UnityPlayer.dll+0145A24C,C,18,44,7C") != 1)
				{
					memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,7C", "float", "1");
				}
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,78", "float", "0");
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,80", "float", "1");
				memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,84", "float", "1");

				SateliteViewEnabled = false;
			}
			
		}

		private void WinTopTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			SpaceCheatModeMainWin.DragMove();
		}

		private void DisabledCheats(object sender, RoutedEventArgs e)
		{
			sb.Stop();
			memory.CloseProcess();
			CheatsEnabled = false;
		}

		private void SabotageLightsButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["SabotageLights"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void SabotageO2Button_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["SabotageO2"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void SabotageCommunicationsButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["SabotageComms"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void SabotageReactorButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["SabotageReactor"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void AllSameColorSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			AllSameColorEnabled = !AllSameColorEnabled;
		}

		private void AboutIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MessageBox.Show("SpaceCheatMode V8 By PoseidonPhoenix \n Huge Thanks To: \n Alizer \n Adaf \n Murzik \n Alixy \n Dev \n Without them this mod wouldn't exist!", "About");
		}

		private void GameVersionIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			MessageBox.Show("Required Among Us Version: 2021.5.25.2e", "GameVersion");
		}

		private void SabotageDoorsButton_Click(object sender, RoutedEventArgs e)
		{
			if (!InfiniteSabotageEnabled)
			{
				memory.WriteMemory((jsonfile["closealldoorsinitiated"]).ToString("X"), "byte", "1");
			}
			else
			{
				MessageBox.Show("If you want to close doors you have to disable the Infinite Sabotage", "Sorry");
			}
		}

		private void LocalPlayerWhiteButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["ColorSelected"]).ToString("X"), "int", "7");
			memory.WriteMemory((jsonfile["changecolorinitiated"]).ToString("X"), "byte", "1");
		}

		private void AllPlayerWhiteButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllSameColorEnabled)
			{
				memory.WriteMemory((jsonfile["AllColor"]).ToString("X"), "int", "7");
				memory.WriteMemory((jsonfile["allsamecolorinitiated"]).ToString("X"), "byte", "1");
			}
		}

		private void RainbowLocalSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!RainbowLPlayer)
			{
				memory.WriteMemory((jsonfile["RainbowOpInitiated"]).ToString("X"), "byte", "1");
				RainbowLPlayer = true;
			}
			else
			{
				memory.WriteMemory((jsonfile["RainbowOpInitiated"]).ToString("X"), "byte", "0");
				RainbowLPlayer = false;
			}
		}

		private void YouTubeChannelPopUpIcon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			Process.Start("https://www.youtube.com/channel/UCs4LNzz7lPr5QjTKd5jKYcw");
		}

		private void SabotageO2Button_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			memory.WriteMemory((jsonfile["RepairO2"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void SabotageCommunicationsButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			memory.WriteMemory((jsonfile["RepairComms"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void SabotageReactorButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			memory.WriteMemory((jsonfile["RepairReactor"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void DisableReportAndMeetingsSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!DisableReportAndMeetings)
			{
				memory.WriteMemory("GameAssembly.dll+681AB0", "byte", "C3");
				DisableReportAndMeetings = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+681AB0", "byte", "55");
				DisableReportAndMeetings = false;
			}
		}

		private void RainbowNameSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!RainbowNameInitiated)
			{
				//memory.WriteMemory("MagicMushroom.dll+6C6D", "byte", "1");
				RainbowNameInitiated = true;
			}
			else
			{
				//memory.WriteMemory("MagicMushroom.dll+6C6D", "byte", "0");
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,4C,30", "float", "1");
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,4C,34", "float", "1");
				memory.WriteMemory("GameAssembly.dll+01C879B8,5C,0,4C,38", "float", "1");
				RainbowNameInitiated = false;
			}
		}

		private void stackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			OpenStoryboard = FindResource("StartingPanelAnimation2") as Storyboard;
			OpenStoryboard.Begin();
			DisableStckP = FindResource("DisableStackPanel") as Storyboard;
			DisableStckP.Begin();
		}

		private void H_PageArrow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!HostPageArrowFlipped)
			{
				RotateH_PageArrow = FindResource("FlipArrow") as Storyboard;
				RotateH_PageArrow.Begin();
				HostPageArrowFlipped = true;
			}
			else
			{
				ResetH_PageArrowRotation = FindResource("ResetArrowRotation") as Storyboard;
				ResetH_PageArrowRotation.Begin();
				HostPageArrowFlipped = false;
			}
		}

		private void H_PageArrowButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!HostPageArrowFlipped)
			{
				RotateH_PageArrow = FindResource("FlipArrow") as Storyboard;
				RotateH_PageArrow.Begin();
				MoveHostpage1 = FindResource("HostGridPart1MoveInitiate") as Storyboard;
				MoveHostpage1.Begin();
				HostPageArrowFlipped = true;
			}
			else
			{
				ResetH_PageArrowRotation = FindResource("ResetArrowRotation") as Storyboard;
				ResetH_PageArrowRotation.Begin();
				ResetHostPage1 = FindResource("HostGridPart1Reset") as Storyboard;
				ResetHostPage1.Begin();
				HostPageArrowFlipped = false;
			}
		}

		private void ArrowTBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!HostPageArrowFlipped)
			{
				RotateH_PageArrow = FindResource("FlipArrow") as Storyboard;
				RotateH_PageArrow.Begin();
				MoveHostpage1 = FindResource("HostGridPart1MoveInitiate") as Storyboard;
				MoveHostpage1.Begin();
				HostPageArrowFlipped = true;
			}
			else
			{
				ResetH_PageArrowRotation = FindResource("ResetArrowRotation") as Storyboard;
				ResetH_PageArrowRotation.Begin();
				ResetHostPage1 = FindResource("HostGridPart1Reset") as Storyboard;
				ResetHostPage1.Begin();
				HostPageArrowFlipped = false;
			}
		}

		private void LockCameraSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!LockCameraEnabled)
			{
				memory.WriteMemory("UnityPlayer.dll+8858F", "bytes", "90 90 90");
				LockCameraEnabled = true;
			}
			else
			{
				memory.WriteMemory("UnityPlayer.dll+8858F", "bytes", "0F 11 01");
				LockCameraEnabled = false;
			}
		}

		private void ChangeAllNames_Click(object sender, RoutedEventArgs e)
		{
			char[] splitted = AllNameInputTextBox.Text.ToCharArray();
			int MoonCounter = 0;
			/*
			if (splitted.Length > 10)
			{
				for (int U = splitted.Length - 1; U > 9; --U)
				{
					int indexToRemove = U;
					splitted = splitted.Where((source, index) => index != indexToRemove).ToArray();
				}
			}
			*/
			for (int B = 0; B < ((splitted.Length * 2 )+ 1); B += 2)
			{
				string address_for_clean_up = (jsonfile["name_container4"] + B).ToString("X");
				memory.WriteMemory(address_for_clean_up, "byte", "00");
			}
			foreach (char ch in splitted)
			{
				string charactbyte = ((byte)ch).ToString("X");
				string full_address_of_name_container = (jsonfile["name_container4"] + MoonCounter).ToString("X");
				memory.WriteMemory(full_address_of_name_container, "byte", charactbyte);
				MoonCounter += 2;
			}
			
			memory.WriteMemory((jsonfile["AllSameNameInitiated"]).ToString("X"), "byte", "1");
		}

		private void CameraZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			mvar = 2.1;

			if (CameraZoomSlider.Value > 0.2 && CameraZoomSlider.Value < 0.35)
				mvar = 2.03;
			if (CameraZoomSlider.Value > 0.35 && CameraZoomSlider.Value < 0.455)
				mvar = 2.07;
			if (CameraZoomSlider.Value >= 0.51 && CameraZoomSlider.Value < 0.537)
				mvar = 2.11;
			if (CameraZoomSlider.Value > 0.535 && CameraZoomSlider.Value < 0.545)
				mvar = 2.118;
			if (CameraZoomSlider.Value > 0.547 && CameraZoomSlider.Value < 0.563)
				mvar = 2.123;
			if (CameraZoomSlider.Value > 0.563 && CameraZoomSlider.Value < 0.574)
				mvar = 2.130;
			if (CameraZoomSlider.Value > 0.574 && CameraZoomSlider.Value < 0.585)
				mvar = 2.135;
			if (CameraZoomSlider.Value > 0.585 && CameraZoomSlider.Value < 0.59)
				mvar = 2.1385;
			if (CameraZoomSlider.Value > 0.59 && CameraZoomSlider.Value < 0.594)
				mvar = 2.1409;
			if (CameraZoomSlider.Value > 0.594 && CameraZoomSlider.Value < 0.5987)
				mvar = 2.143;
			if (CameraZoomSlider.Value > 0.5987 && CameraZoomSlider.Value <= 0.6)
				mvar = 2.146;

			AmongUsScalingUIAddressesValue = (Math.Tan(mvar * CameraZoomSlider.Value) * Math.Tan(mvar * CameraZoomSlider.Value)) + 1;
			memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,84", "float", AmongUsScalingUIAddressesValue.ToString());
			memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,80", "float", AmongUsScalingUIAddressesValue.ToString());
			memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,78", "float", (CameraZoomSlider.Value).ToString());
			memory.WriteMemory("UnityPlayer.dll+0145A24C,C,18,44,7C", "float", "0");
		}


		private void EveryoneGhostSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!AllGhostsEnabled)
			{
				memory.WriteMemory("GameAssembly.dll+53C9F0", "bytes", "90 90 90 90");
				AllGhostsEnabled = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+53C9F0", "bytes", "0F B6 40 2D");
				AllGhostsEnabled = false;
			}
		}

		private void AllPlayersRainbowSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!AllPlayersRainbow)
			{
				memory.WriteMemory((jsonfile["AllPlayersRainbowInitiated"]).ToString("X"), "byte", "01");
				AllPlayersRainbow = true;
			}
			else
			{
				memory.WriteMemory((jsonfile["AllPlayersRainbowInitiated"]).ToString("X"), "byte", "00");
				AllPlayersRainbow = false;
			}
		}

		private void DisableVotingSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!DisableVoting)
			{
				memory.WriteMemory("GameAssembly.dll+7B7640", "bytes", "C3");
				DisableVoting = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+7B7640", "bytes", "55");
				DisableVoting = false;
			}
		}

		private void EveryoneGhostServerSidedSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!AllGhostsServerSidedEnabled)
			{
				memory.WriteMemory("GameAssembly.dll+1652EE2", "bytes", "90 90 90 90");
				memory.WriteMemory("GameAssembly.dll+17B4DFD", "bytes", "80 7E 2D 01");
				AllGhostsServerSidedEnabled = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+1652EE2", "bytes", "0F B6 40 2D");
				memory.WriteMemory("GameAssembly.dll+17B4DFD", "bytes", "80 7E 2D 00");
				AllGhostsServerSidedEnabled = false;
			}
		}

		private void SabotageLightsButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			memory.WriteMemory((jsonfile["RepairLights"]).ToString("X"), "byte", "1");
			memory.WriteMemory((jsonfile["sabotageinitiated"]).ToString("X"), "byte", "1");
		}

		private void FakeCamerasSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!FakeCamerasEnabled)
			{
				memory.WriteMemory((jsonfile["FakeCamerasEnabled"]).ToString("X"), "byte", "1");
				FakeCamerasEnabled = true;
			}
			else
			{
				memory.WriteMemory((jsonfile["DisableFakeCamera"]).ToString("X"), "byte", "1");
				FakeCamerasEnabled = false;
			}
		}

		private void EnableSabotageSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!EnableSabotage)
			{
				//GameAssembly.dll+12E9504
				memory.WriteMemory("GameAssembly.dll+6EFAC7", "bytes", "90 90 90 90");
				memory.WriteMemory("GameAssembly.dll+6EFCD4", "bytes", "90 90 90 90");
				EnableSabotage = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+6EFAC7", "bytes", "80 78 2C 00");
				memory.WriteMemory("GameAssembly.dll+6EFCD4", "bytes", "80 78 2C 00");
				EnableSabotage = false;
			}
		}

		private void DisableLightsButton_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["DisableLights"]).ToString("X"), "byte", "1");
		}

		private void DisableLightsButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
		{
			memory.WriteMemory((jsonfile["Re_enableLights"]).ToString("X"), "byte", "1");
		}

		private void UnlockChatSwitch_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!isChatUnlocked)
			{
				memory.WriteMemory("GameAssembly.dll+01B874C4,AC", "int", "1");
				isChatUnlocked = true;
			}
			else
			{
				memory.WriteMemory("GameAssembly.dll+01B874C4,AC", "int", "2");
				isChatUnlocked = false;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			memory.WriteMemory((jsonfile["CompleteTasksActive"]).ToString("X"), "byte", "1");
		}

		private void ColorPicker1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			UIChangesEnabled = true;
		}

		private void ColorPicker2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			UIChangesEnabled = true;
		}
	}
}