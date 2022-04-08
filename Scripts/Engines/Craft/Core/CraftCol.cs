// szczaw :: 2013.01.13 :: refaktoryzacja - zamiast 3 plików i 180 linii kodu, jeden plik z 80 liniami...
// ... i na dodatek zaimplementowane dodatkowe intefejsy. 
using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{

    public class CraftResCol : CraftCol<CraftRes>
    {
    }

    public class CraftSkillCol : CraftCol<CraftSkill>
    {
    }

    public class CraftItemCol : CraftCol<CraftItem>
    {
        public CraftItem SearchFor( Type type )
        {
            return base.SearchFor(type, this, false);
        }

        public CraftItem SearchForSubclass( Type type )
        {
            return base.SearchFor(type, this, true);
        }
    }


    public class CraftGroupCol : CraftCol<CraftGroup>
    {

        public int SearchFor( TextDefinition groupName )
        {
            return this.FindIndex(craftGroup =>
                                           (craftGroup.NameNumber != 0 && craftGroup.NameNumber == groupName.Number)
                                           || (craftGroup.NameString != null && craftGroup.NameString == groupName.String)
                                 );

        }
    }

    public class CraftSubResCol : CraftCol<CraftSubRes>
    {
        public bool Init { get; set; }
        public Type ResType { get; set; }
        public string NameString { get; set; }
        public int NameNumber { get; set; } 

        public CraftSubResCol()
        {
            Init = false;
        }

        public CraftSubRes SearchFor( Type type )
        {
            return base.SearchFor(type, this, false);
        }
    }



    public class CraftCol<T> : List<T>
    {
        public virtual int Add( T item )
        {
            base.Add(item);

            return base.IndexOf(item);
        }

        public virtual void Remove( int index )
        {
            base.RemoveAt(index);
        }

        public virtual T GetAt( int index )
        {
            return this[index];
        }

        public virtual CraftItem SearchFor( Type type, CraftCol<CraftItem> col, bool subclass )
        {
            List<CraftItem> list = new List<CraftItem>();
            foreach (CraftItem c in col)
            {
                if (c.ItemType == type)
                {
                    list.Add(c);
                }
                else
                {
                    if (subclass)
                    {
                        if (type.IsSubclassOf(c.ItemType))
                        {
                            list.Add(c);
                        }
                    }
                }
            }
            if(list.Count > 0)
                return list[0];
            else
                return default(CraftItem);
        }

        public virtual CraftSubRes SearchFor(Type type, CraftCol<CraftSubRes> col, bool subclass)
        {
            List<CraftSubRes> list = new List<CraftSubRes>();
            foreach (CraftSubRes c in col)
            {
                if (c.ItemType == type)
                {
                    list.Add(c);
                }
                else
                {
                    if (subclass)
                    {
                        if (type.IsSubclassOf(c.ItemType))
                        {
                            list.Add(c);
                        }
                    }
                }
            }
            if (list.Count > 0)
                return list[0];
            else
                return default(CraftSubRes);
        }
    }
}