using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSCardKiosks
{
  
    class CameraHelper
    {
        public const int CW_ERR_CameraNotOpen = 9000;       //摄像头未打开
        public const int CW_ERR_CameraOpenError = 9001;     //摄像头打开失败
        public const int CW_ERR_CameraOpenAdy = 9002;       //摄像头已经打开
        public const int CW_ERR_InitSDK = 9100;     //初始化SDK失败

        //活体状态
        public const int CW_LivenessOK = 0;         //活体并正常获得最佳人脸
        public const int CW_LivenessFailure = 9301;     //未符合活体要求
        public const int CW_LivenessNOFace = 9302;      //未检测到人脸
        public const int CW_LivenessNisNoFace = 9303;       //红外未检测到人脸
        public const int CW_LivenessLostFace = 9304;        //人脸丢失
        public const int CW_LivenessManyFace = 9305;        //检测到多人
        public const int CW_LivenessFaceChange = 9307;      //检测到换人
        public const int CW_GetBestFaceErr = 117;       //获取最佳人脸失败

        public const int EnumLocalNetError = 800;       //网络异常。
        public const int EnumLocalNetErrorUrl = 803;        //URL格式不正确。
        public const int EnumLocalNetErrorSerAddr = 806;        //无法解析人脸识别服务主机地址。
        public const int EnumLocalNetErrorConSer = 807;     //无法连接到人脸识别服务主机。
        public const int EnumLocalNetErrorTimeOut = 828;        //访问人脸识别服务主机超时。
        public const int EnumLocalNetErrorActData = 890;        //无法解析人脸识别服务主机返回数据。

        public const int EnumLocalNoFace = 1100;		//未检测到人脸

        
    }
}
