using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSCardKiosks
{
    class GetSreenDPI
    {
        private double dWidthRate;
        private double dHeightRate;
        public GetSreenDPI()
        {
            int iWidth = Convert.ToInt32(DealAppConfig.GetAppSettingsValue("屏幕尺寸").Split('*')[0]);
            int iHeight = Convert.ToInt32(DealAppConfig.GetAppSettingsValue("屏幕尺寸").Split('*')[1]);
            dWidthRate = iWidth / 1920.00;
            dHeightRate = iHeight / 1080.00;
        }
        public double DWidthRate { get => dWidthRate; }
        public double DHeightRate { get => dHeightRate; }


    }
}
