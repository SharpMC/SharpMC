using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccidentalNoise
{
    public class TranslatedDomain : ModuleBase
    {
        ModuleBase tx, ty;

        public TranslatedDomain(ModuleBase source, ModuleBase tx, ModuleBase ty)
        {
            this.Source = source;
            this.tx = tx;
            this.ty = ty;
        }

      public override double  Get(double x, double y)
      {
          double m_ax = tx == null ? 0 : tx.Get(x, y);
          double m_ay = ty == null ? 0 : ty.Get(x, y);

 	      return Source.Get(x + m_ax, y + m_ay);
      }
    }
}
