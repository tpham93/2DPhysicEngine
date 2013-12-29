using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicEngine.ObjectData.Objects
{
    class MaterialData
    {
        float density;
        float restitution;

        public float Density
        {
            get { return density; }
        }
        public float Restitution
        {
            get { return restitution; }
        }

        public MaterialData(float density, float restitution)
        {
            this.density = density;
            this.restitution = restitution;
        }

        private static MaterialData stone = new MaterialData(2.6f, 0.1f);

        public static MaterialData Stone
        {
            get { return MaterialData.stone; }
        }





    }
}
