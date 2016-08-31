using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circulation_Toolkit
{
    /// <summary>
    /// Main Entity class that stores information for the Circulation Environment
    /// </summary>
    class Entity
    {
        private Profile _profile;
        private Dictionary<int, int> _pos;

        public Entity(Profile profile)
        {
            this._profile = profile;
            this._pos = new Dictionary<int, int>();
        }

        public string Type
        {
            get
            {
                return this._profile.Type;
            }
            set
            {
                this._profile.Type = value;
            }
        }

        public string Name
        {
            get
            {
                return this._profile.Name;
            }
            set
            {
                this._profile.Name = value;
            }
        }

        public Profile Profile
        {
            get
            {
                return this._profile;
            }
        }

        public int GetPosition(int gen)
        {
            if (this._pos.ContainsKey(gen))
            {
                return this._pos[gen];
            }
            else
            {
                return -1;
            }
        }

        public void SetPosition(int pos, int gen = -1)
        {
            if (gen == -1)
            {
                this._pos[this._pos.Last().Key] = pos;
            }
            else
            {
                this._pos[gen] = pos;         
            }
        }

        public bool HasAttribute(string attribute)
        {
            return this.Profile.HasAttribute(attribute);
        }

        public string GetAttribute(string attribute)
        {
            if (this.HasAttribute(attribute))
            {
                return this.Profile.Attributes[attribute];
            }
            else
            {
                return "None";
            }
        }

        public void SetAttribute(string attribute, string value)
        {
            this.Profile.Attributes[attribute] = value;
        }

        public bool HasValue(string value)
        {
            return this.Profile.HasValue(value);
        }
    }

}
