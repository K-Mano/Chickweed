using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Chickweed
{
    public class Utilities
    {
        public string[] SubstringAtCount(string self, int count)
        {
            var result = new List<string>();
            var length = (int)Math.Ceiling((double)self.Length / count);

            for (int i = 0; i < length; i++)
            {
                int start = count * i;
                if (self.Length <= start)
                {
                    break;
                }
                if (self.Length < start + count)
                {
                    result.Add(self.Substring(start));
                }
                else
                {
                    result.Add(self.Substring(start, count));
                }
            }

            return result.ToArray();
        }

        public int[] DateCount(int year, int month, int addyear, int addmonth)
        {
            int[] output = new int[2];

            output[0] = year + addyear;
            output[1] = month + addmonth;

            if (output[1] >= 12)
            {
                output[0] += output[1] / 12;
                output[1] = output[1] % 12;
            }

            return output;
        }
    }
}
