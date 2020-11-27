using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JSCardKiosks
{
    public class CardBaseData
    {/// <summary>
     /// 省人员识别号
     /// </summary>
        string baz805="";//省人员识别号
        string aac058="";//证件类型
        string aac147="";//证件号码
        string aac003="";//姓名
        string aac004="";//性别
        string aac161="";//国家或地区
        string aae005="";//移动电话
        string aae006="";//联系地址
        string image="";//照片
        string imageCode="";//照片
        string aaz502="";// 卡状态
        string aac059="";//   证件有效期截止日期
        string aae008="";//  银行类别
        string baz030="";//网点制卡批次
        string baz033="";// 制卡批次号
        string aab301="";//行政区划
        string aac002="";//社会保障号码
        string aac005="";//民族
        string aac006="";//出生日期
        string baz103="";//姓名扩展
        string aac025="";//出生地
        string aac500="";//9位卡号
        string aaz503="";// 发卡日期
        string aaz501="";//  卡识别码
        string bac025="";//职业

        public CardBaseData(string name, string idNur)
        {
            aac003 = name;
            aac147 = idNur;
        }
        /// <summary>
        /// 省人员识别号
        /// </summary>
        public string Baz805 { get => baz805; set => baz805 = value; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string Aac058 { get => aac058; set => aac058 = value; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string Aac147 { get => aac147; set => aac147 = value; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Aac003 { get => aac003; set => aac003 = value; }
        /// <summary>
        /// 性别
        /// </summary>
        public string Aac004 { get => aac004; set => aac004 = value; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Aac161 { get => aac161; set => aac161 = value; }
        /// <summary>
        /// 移动电话
        /// </summary>
        public string Aae005 { get => aae005; set => aae005 = value; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Aae006 { get => aae006; set => aae006 = value; }
        /// <summary>
        /// 照片
        /// </summary>
        public string Image { get => image; set => image = value; }
        /// <summary>
        /// 卡状态
        /// </summary>
        public string Aaz502 { get => aaz502; set => aaz502 = value; }
        /// <summary>
        /// 证件有效期截止日期
        /// </summary>
        public string Aac059 { get => aac059; set => aac059 = value; }
        /// <summary>
        /// 银行类别
        /// </summary>
        public string Aae008 { get => aae008; set => aae008 = value; }
        /// <summary>
        /// 网点制卡批次
        /// </summary>
        public string Baz030 { get => baz030; set => baz030 = value; }
        /// <summary>
        /// 行政区划
        /// </summary>
        public string Aab301 { get => aab301; set => aab301 = value; }
        /// <summary>
        /// 社会保障号码
        /// </summary>
        public string Aac002 { get => aac002; set => aac002 = value; }
        /// <summary>
        /// 民族
        /// </summary>
        public string Aac005 { get => aac005; set => aac005 = value; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public string Aac006 { get => aac006; set => aac006 = value; }
        /// <summary>
        /// 姓名扩展
        /// </summary>
        public string Baz103 { get => baz103; set => baz103 = value; }
        /// <summary>
        /// 出生地
        /// </summary>
        public string Aac025 { get => aac025; set => aac025 = value; }
        /// <summary>
        /// 9位卡号
        /// </summary>
        public string Aac500 { get => aac500; set => aac500 = value; }
        /// <summary>
        /// 发卡日期
        /// </summary>
        public string Aaz503 { get => aaz503; set => aaz503 = value; }
        /// <summary>
        /// 卡识别码
        /// </summary>
        public string Aaz501 { get => aaz501; set => aaz501 = value; }
        /// <summary>
        /// 职业
        /// </summary>
        public string Bac025 { get => bac025; set => bac025 = value; }
        /// <summary>
        /// 制卡批次号
        /// </summary>
        public string Baz033 { get => baz033; set => baz033 = value; }
        public string ImageCode { get => imageCode; set => imageCode = value; }
    }
}
