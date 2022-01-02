using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//외부 라이브러리
using ED_getPLC_001.lib.Focas; //화낙fanuc Focas 라이브러리


namespace ED_getPLC_001.Facility_Controll
{
	class ED_Fanuc : Elumi_Fanuc
	{
		public ushort facilityHandle { get; set; }
		public string Tool_PotNumber { get; set; }
		public string Tool_PTN { get; set; }
		public string Tool_Life_Counter_Alarm { get; set; }
		public string Tool_Life_Counter_Warning { get; set; }
		public string Tool_Life_Counter_Current { get; set; }

	}

	class Elumi_Fanuc
	{
		//화낙설비의 데이터를 컨트롤 할때 handle을 취득해야한다. 핸들 취득함수
		public ushort fanuc_getHandle(string ip)
		{
			ushort handle;
			short ret;

			ret = Focas1.cnc_allclibhndl3(ip, 8193, 1, out handle);

			if (ret == -16)
			{
				handle = 0;
				Console.WriteLine("해당 설비는 연결 불가능입니다.");
			}
			
			return handle;
		}

		public string sample(ushort h, ushort start, ushort end, short size)
		{
			short ret, idx;
			short adr_type, data_type;
			ushort length;
			string returnValue = "";

			adr_type = 9;                     // In case that kind of PMC address is G
			data_type = 2;                    // In case that type of PMC data is Byte
			length = (ushort)(8 + (end - start + 1) * 4);
			Focas1.IODBPMC iODBPMC = new Focas1.IODBPMC();

			ret = Focas1.pmc_rdpmcrng(h, adr_type, data_type, start, end, length, iODBPMC);

			if (ret == Focas1.EW_OK)
			{

				for (idx = size; idx >= 0; idx--)
				{
					if (iODBPMC.ldata[idx].ToString() != "0")
					{
						returnValue += Convert.ToString(int.Parse(iODBPMC.ldata[idx].ToString()));
					}
				}
				if (returnValue != "")
				{
					//Console.WriteLine(returnValue);
				}
			}
			else
			{
				returnValue = "fail";
				Console.WriteLine("ERROR!({0})", ret);
			}
			return (returnValue);
		}

		public string rdCurrent(ushort handle)
		{
			short cur;
			Focas1.cnc_rdcurrent(handle, out cur);

			return cur.ToString();
		}

		public int rdTimer(ushort handle)
		{
			short ret;
			Focas1.IODBTIME Cycle_time = new Focas1.IODBTIME();

			ret = Focas1.cnc_rdtimer(handle, 3, Cycle_time);

			return Cycle_time.msec;
		}

		public string rdaxis(ushort handle)
		{
			short ret;
			string S_axis = "";
			Focas1.ODBAXIS axis = new Focas1.ODBAXIS();
			ret = Focas1.cnc_absolute2(handle, 1, 8, axis);
			S_axis += "--x:" + axis.data[0];

			ret = Focas1.cnc_absolute2(handle, 2, 8, axis);
			S_axis += "--y:" + axis.data[0];

			ret = Focas1.cnc_absolute2(handle, 3, 8, axis);
			S_axis += "--z:" + axis.data[0];

			return S_axis;
		}

		public string prgnum(ushort handle)
		{
			Focas1.ODBPRO getprgno = new Focas1.ODBPRO();
			Focas1.cnc_rdprgnum(handle, getprgno);

			return getprgno.mdata.ToString();
		}
	}
}
