using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nelderim;

namespace Nelderim.Speech
{
    class SpeechInfo : NExtensionInfo
    {
        private KnownLanguages m_LanguagesKnown;
        private SpeechLang m_LanguageSpeaking;

        public SpeechInfo()
        {
            m_LanguagesKnown = new KnownLanguages();
        }

        public KnownLanguages LanguagesKnown { get { return m_LanguagesKnown; } set { m_LanguagesKnown = value; } }
        public SpeechLang LanguageSpeaking { get { return m_LanguageSpeaking; } set { m_LanguageSpeaking = value; } }

        public override void Serialize( GenericWriter writer )
        {
            writer.Write( (int)0 ); //version
            writer.Write( (int)m_LanguagesKnown.Value );
            writer.Write( (int)m_LanguageSpeaking );
        }

        public override void Deserialize( GenericReader reader )
        {
            int version = 0;
            if (Fix)
                version = reader.ReadInt(); //version
            m_LanguagesKnown = new KnownLanguages((SpeechLang)reader.ReadInt());
            m_LanguageSpeaking = (SpeechLang)reader.ReadInt();

            if ( m_LanguagesKnown == null ) m_LanguagesKnown = new KnownLanguages();
        }
    }
}
