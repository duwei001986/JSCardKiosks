using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSCardKiosks
{
    class DataHaseTable
    {
        public static string GetNationCode(string nation)
        {
            Hashtable ht = new Hashtable(); //创建一个Hashtable实例
            ht.Add("汉族", "01");
            ht.Add("蒙古族", "02");
            ht.Add("回族", "03");
            ht.Add("藏族", "04");
            ht.Add("维吾尔族", "05");
            ht.Add("苗族", "06");
            ht.Add("彝族", "07");
            ht.Add("壮族", "08");
            ht.Add("布依族", "09");
            ht.Add("朝鲜族", "10");
            ht.Add("满族", "11");
            ht.Add("侗族", "12");
            ht.Add("瑶族", "13");
            ht.Add("白族", "14");
            ht.Add("土家族", "15");
            ht.Add("哈尼族", "16");
            ht.Add("哈萨克族", "17");
            ht.Add("傣族", "18");
            ht.Add("黎族", "19");
            ht.Add("傈僳族", "20");
            ht.Add("佤族", "21");
            ht.Add("畲族", "22");
            ht.Add("高山族", "23");
            ht.Add("拉祜族", "24");
            ht.Add("水族", "25");
            ht.Add("东乡族", "26");
            ht.Add("纳西族", "27");
            ht.Add("景颇族", "28");
            ht.Add("柯尔克孜族", "29");
            ht.Add("土族", "30");
            ht.Add("达斡尔族", "31");
            ht.Add("仫佬族", "32");
            ht.Add("羌族", "33");
            ht.Add("布朗族", "34");
            ht.Add("撒拉族", "35");
            ht.Add("毛难族", "36");
            ht.Add("仡佬族", "37");
            ht.Add("锡伯族", "38");
            ht.Add("阿昌族", "39");
            ht.Add("普米族", "40");
            ht.Add("塔吉克族", "41");
            ht.Add("怒族", "42");
            ht.Add("乌孜别克族", "43");
            ht.Add("俄罗斯族", "44");
            ht.Add("鄂温克族", "45");
            ht.Add("崩龙族", "46");
            ht.Add("保安族", "47");
            ht.Add("裕固族", "48");
            ht.Add("京族", "49");
            ht.Add("塔塔尔族", "50");
            ht.Add("独龙族", "51");
            ht.Add("鄂伦春族", "52");
            ht.Add("赫哲族", "53");
            ht.Add("门巴族", "54");
            ht.Add("珞巴族", "55");
            ht.Add("基诺族", "56");
            ht.Add("其他", "57");
            ht.Add("外国血统中国人士", "58");
            return (string)ht[nation];
        }
    }
}
