using System.ComponentModel;

namespace PGK.Domain.Journal
{
    public enum JournalEvaluation
    {
       [Description("2")]
       HAS_2,
       [Description("3")]
       HAS_3,
       [Description("4")]
       HAS_4,
       [Description("5")]
       HAS_5,
       [Description("н")]
       HAS_H
    }

    public class Evaluation
    {
        public static int ToInt(JournalEvaluation evaluation)
        {
            return evaluation switch
            {
                JournalEvaluation.HAS_2 => 2,
                JournalEvaluation.HAS_3 => 3,
                JournalEvaluation.HAS_4 => 4,
                JournalEvaluation.HAS_5 => 5,
                _ => 0
            };
        }
    }
}
