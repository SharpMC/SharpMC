using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpMC.Classes;
using SharpMC.Items;

namespace SharpMC.Networking.Packages
{
	class UseEntity : Package<UseEntity>
	{
		public UseEntity(ClientWrapper client)
			: base(client)
		{
			ReadId = 0x02;
		}

		public UseEntity(ClientWrapper client, MSGBuffer buffer)
			: base(client, buffer)
		{
			ReadId = 0x02;
		}

		public override void Read()
		{
			var target = Buffer.ReadVarInt();
			var type = Buffer.ReadVarInt();
			float targetX;
			float targetY;
			float targetZ;

			if (type == 2)
			{
				//Interact at
				targetX = Buffer.ReadFloat();
				targetY = Buffer.ReadFloat();
				targetZ = Buffer.ReadFloat();
			}
			else if (type == 1)
			{
				//Player hit
				var targ = Client.Player.CurrentLevel.GetPlayer(target);
				if (targ != null)
				{
					targ.HealthManager.TakeHit(Client.Player, 1, DamageCause.EntityAttack);
				}
			}
			else if (type == 0)
			{
				//Interact

				//I guess this is for stuff like villagers & horses for openening inventory's?
				//Or maybe to get on a horse or somethin
			}
		}
	}
}
