using System.Configuration;
using System.Threading;

using dbControl;

using ED_getPLC_001.Facility_Controll;

using ED_getPLC_001.lib.Focas;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//ToolLimit

namespace rdCurrent
{
	class Program
	{
		static void Main(string[] args)
		{
			ED_Fanuc pot1 = new ED_Fanuc();
			#region 데이터
			string[,] A_Alarm = new string[8, 30];
			string[,] A_Warning = new string[8, 30];			
			string[,] A_Current = new string[8, 30];

			Dictionary<string, int> ToolAlarmList = new Dictionary<string, int>();

			Elumi_Fanuc fanucfn = new Elumi_Fanuc();

			for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++) //초기값 설정
			{
				pot1.facilityHandle = fanucfn.fanuc_getHandle(ConfigurationManager.AppSettings.Get(i).ToString());
				Console.WriteLine($"---설비 : {(i + 1)} ---IP : {ConfigurationManager.AppSettings.Get(i).ToString()}----");
				int j = 0;
				int memorystart = 5604 + 60 * j;
				string PTN = fanucfn.sample(pot1.facilityHandle, (ushort)memorystart, (ushort)(memorystart + 5), 3);

				Console.WriteLine("--PTN(툴정보) : " + PTN);

				Console.WriteLine($"--CycleTime(가공시간) : {fanucfn.rdTimer(pot1.facilityHandle)}");
				Console.WriteLine($"--CycleAxis(위치좌표) : {fanucfn.rdaxis(pot1.facilityHandle)}");

				Focas1.cnc_freelibhndl(pot1.facilityHandle);
				Console.WriteLine("\n");
			}
			Console.ReadKey();
			#region keep
			/*
			bool t = false;

			while (t)
			{
				for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
				{
					pot1.facilityHandle = fanucfn.fanuc_getHandle(ConfigurationManager.AppSettings.Get(i).ToString());
					//5616 + 60*x
					Console.WriteLine("-----------------------------VM0" + (i + 1));
					for (int j = 0; j <= 29; j++)
					{
						int memorystart = 5604 + 60 * j;
						Console.WriteLine("--" + j + "------" + memorystart);
						string PTN = fanucfn.sample(pot1.facilityHandle, (ushort)memorystart, (ushort)(memorystart + 5), 3);

						memorystart += 4;
						string C_Alarm = fanucfn.sample(pot1.facilityHandle, (ushort)memorystart, (ushort)(memorystart + 5), 3);

						memorystart += 4;
						string C_Warning = fanucfn.sample(pot1.facilityHandle, (ushort)memorystart, (ushort)(memorystart + 5), 3);

						memorystart += 4;
						string C_Current = fanucfn.sample(pot1.facilityHandle, (ushort)memorystart, (ushort)(memorystart + 5), 3);

						if (C_Alarm != A_Alarm[i, j])
						{
							UpdateToolAlarm($"VM0{(i + 1)}", PTN, C_Alarm);
							A_Alarm[i, j] = C_Alarm;
							UpdateToolWarning($"VM0{(i + 1)}", PTN, C_Warning);
							A_Warning[i, j] = C_Warning;
						}

						if (C_Warning != A_Warning[i, j])
						{
							UpdateToolWarning($"VM0{(i + 1)}", PTN, C_Warning);
							A_Warning[i, j] = C_Warning;
						}

						if (C_Current != A_Current[i, j])
						{
							UpdateToolCurrent($"VM0{(i + 1)}", PTN, C_Current);
							A_Current[i, j] = C_Current;
						}
						try
						{
							if (int.Parse(A_Warning[i, j]) < int.Parse(A_Current[i, j]))
							{
								if (ToolAlarmList.ContainsKey((i + "-" + PTN)))
								{
									if (ToolAlarmList[(i + "-" + PTN)] != 1)
									{
										ToolAlarmList.Add(i + "-" + PTN, 1);
										insertToolAlarm($"VM0{(i + 1)}", PTN, "Warning");
									}
								}
								else
								{
									ToolAlarmList.Add(i + "-" + PTN, 1);
									insertToolAlarm($"VM0{(i + 1)}", PTN, "Warning");
								}
							}
							else if(int.Parse(A_Alarm[i, j]) < int.Parse(A_Current[i, j]))
							{
								if(ToolAlarmList.ContainsKey((i + "-" + PTN)))
								{
									if (ToolAlarmList[(i + "-" + PTN)] != 2)
									{
										ToolAlarmList[(i + "-" + PTN)] = 2;
										insertToolAlarm($"VM0{(i + 1)}", PTN, "Warning");
									}
								}
								else
								{
									ToolAlarmList.Add(i + "-" + PTN, 2);
									insertToolAlarm($"VM0{(i + 1)}", PTN, "Warning");
								}
							}
							else
							{
								if (ToolAlarmList.ContainsKey((i + "-" + PTN)))
								{
									ToolAlarmList.Remove((i + "-" + PTN));
								}
							}
						}
						catch(Exception ee)
						{
							Console.WriteLine("PTN error: " + i + "-" + PTN + "\n" + ee);
						}

						Console.WriteLine("PTN : " + PTN + " -- Current : " + C_Current);
					}
					Focas1.cnc_freelibhndl(pot1.facilityHandle);
				}
				//t = false;
				Thread.Sleep(4000);
			}

			void insertToolAlarm(string 설비번호, string PTN, string AlarmType)
			{
				string enddate = DateTime.Now.ToString("yyyyMMddHHmmss");
				string basedate;
				string shiftcd;
				if (enddate.Substring(8, 1) == "0")
				{
					if (int.Parse(enddate.Substring(9, 5)) < 70000)
					{
						basedate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");//ens
						shiftcd = "ENS";
					}
					else
					{
						basedate = enddate.Substring(0, 8);//eds
						shiftcd = "EDS";
					}
				}
				else
				{
					if (int.Parse(enddate.Substring(8, 6)) > 190000)//19
					{
						shiftcd = "ENS";
					}
					else
					{
						shiftcd = "EDS";
						basedate = enddate.Substring(0, 8);//ens 해당로우는 밖으로 뺀다
					}
					basedate = enddate.Substring(0, 8);//ens 해당로우는 밖으로 뺀다
				}

				dbAccess_InsertUpdate PmcInsert = new dbAccess_InsertUpdate()
				{
					Query = $"Insert into Facility_ToolAlarm(FacilityNo, PTN, AlarmType, BaseDate, ShiftCd, EventDateTime) Values('{설비번호}', '{PTN}', '{AlarmType}', '{basedate}', '{shiftcd}', '{enddate}' )"

				};
				PmcInsert.InsertUpdate();
			}


			void deleteToolAlarm()
			{
				dbAccess_InsertUpdate DeleteLimit = new dbAccess_InsertUpdate()
				{
					Query = "delete dbo.Facility_ToolLimit"

				};
				DeleteLimit.InsertUpdate();
			}

			void PTNSetting(string 설비번호, int Potno, string PTN)
			{
				dbAccess_InsertUpdate PmcInsert = new dbAccess_InsertUpdate()
				{
					Query = $"Insert into Facility_ToolLimit(FacilityNo, PotNo,PTN) Values('{설비번호}', {Potno},'{PTN}')"
				};
				PmcInsert.InsertUpdate();
			}

			void UpdateToolAlarm(string 설비번호, string PTN, string 가공시간)
			{
				dbAccess_InsertUpdate PmcInsert = new dbAccess_InsertUpdate()
				{
					Query = $"Update Facility_ToolLimit set AlarmCurrent = '{가공시간}', EventTime ='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' Where FacilityNo = '{설비번호}' and PTN = '{PTN}'"
				};
				PmcInsert.InsertUpdate();
			}

			void UpdateToolWarning(string 설비번호, string PTN, string 가공시간)
			{
				dbAccess_InsertUpdate PmcInsert = new dbAccess_InsertUpdate()
				{
					Query = $"Update Facility_ToolLimit set WarningCurrent = '{가공시간}', EventTime ='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' Where FacilityNo = '{설비번호}' and PTN = '{PTN}'"
				};
				PmcInsert.InsertUpdate();
			}

			void UpdateToolCurrent(string 설비번호, string PTN, string 가공시간)
			{
				dbAccess_InsertUpdate PmcInsert = new dbAccess_InsertUpdate()
				{
					Query = $"Update Facility_ToolLimit set PTNCurrent = {가공시간}, EventTime ='{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' Where FacilityNo = '{설비번호}' and PTN = '{PTN}'"
				};
				PmcInsert.InsertUpdate();
			}
			*/
			#endregion
		}
		#endregion
	}
}
