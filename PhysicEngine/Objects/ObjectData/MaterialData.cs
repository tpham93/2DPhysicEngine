using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicEngine.ObjectData.Objects
{
    class MaterialData
    {
        private float density;
        private float restitution;
        private float staticFriction;
        private float dynamicFriction;


        public float Density
        {
            get { return density; }
        }
        public float Restitution
        {
            get { return restitution; }
        }
        public float StaticFriction
        {
            get { return staticFriction; }
        }
        public float DynamicFriction
        {
            get { return dynamicFriction; }
        }

        public MaterialData(float density, float restitution, float staticFriction, float dynamicFriction)
        {
            this.density = density;
            this.restitution = restitution;
            this.staticFriction = staticFriction;
            this.dynamicFriction = dynamicFriction;
        }

        private static MaterialData stone = new MaterialData(2.6f, 0.1f,0.05f,0.03f);

        public static MaterialData Stone
        {
            get { return MaterialData.stone; }
        }





    }
}
