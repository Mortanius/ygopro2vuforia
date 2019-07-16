﻿using UnityEngine;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections.Generic;

public class Menu : WindowServantSP 
{
    //GameObject screen;
    public override void initialize()
    {
        string hint = File.ReadAllText("config/hint.conf");
        createWindow(Program.I().new_ui_menu);
        UIHelper.registEvent(gameObject, "setting_", onClickSetting);
        UIHelper.registEvent(gameObject, "deck_", onClickSelectDeck);
        UIHelper.registEvent(gameObject, "online_", onClickOnline);
        UIHelper.registEvent(gameObject, "replay_", onClickReplay);
        UIHelper.registEvent(gameObject, "single_", Program.gugugu);
        UIHelper.registEvent(gameObject, "ai_", Program.gugugu);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        UIHelper.registEvent(gameObject, "download_", onClickDownload);
        UIHelper.getByName<UILabel>(gameObject, "version_").text = Config.VERSION;
        (new Thread(up)).Start();
    }

    public override void show()
    {
        base.show();
        Program.charge();
    }

    public override void hide()
    {
        base.hide();
    }

    static int Version = 0;
    string upurl = "";
    public static string upurl_ = "";
    void up()
    {
        try
        {
            ServicePointManager.ServerCertificateValidationCallback = HttpDldFile.MyRemoteCertificateValidationCallback;//支持https
            string url = "https://api.ygo2019.xyz/ygopro2/android_ver.txt";
            WebClient wc = new WebClient();
            Stream s = wc.OpenRead(url);
            StreamReader sr = new StreamReader(s, Encoding.UTF8);
            string result = sr.ReadToEnd();
            sr.Close();
            s.Close();
            string[] lines = result.Replace("\r", "").Split("\n");
            if (lines[0] != Program.GAME_VERSION)
            {
                upurl = lines[1];
            }

            /*
            if (lines.Length > 0)
            {
                string[] mats = lines[0].Split(":.:");
                if (mats.Length == 2)
                {
                    if (Version.ToString() != mats[0])
                    {
                        upurl = mats[1];
                    }
                }
            }
            */
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "RMSshow_onlyYes")
        {
            if (result[0].value == "yes")
            {
                Application.OpenURL(upurl);
            }
        }
        if (hashCode == "onCheckUpgrade")
        {
            if (result[0].value == "yes")
            {
                Application.OpenURL(upurl_);
            }
        }
    }

    bool outed = false;
    public override void preFrameFunction()
    {
        base.preFrameFunction();
        if (upurl != "" && outed == false)
        {
            outed = true;
            RMSshow_yesOrNo("RMSshow_onlyYes", InterString.Get("发现更新!@n是否要下载更新？"), new messageSystemValue { hint = "yes", value = "yes" }, new messageSystemValue { hint = "no", value = "no" });
        }
        Menu.checkCommend();
    }

    void onClickExit()
    {
        Program.I().quit();
        Program.Running = false;
        TcpHelper.SaveRecord();
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE) // IL2CPP 使用此方法才能退出
        Application.Quit();
#else
        Process.GetCurrentProcess().Kill();
#endif
    }

    void onClickOnline()
    {
        Program.I().shiftToServant(Program.I().selectServer);
    }

    void onClickAI()
    {
        Program.I().shiftToServant(Program.I().aiRoom);
    }

    void onClickPizzle()
    {
        Program.I().shiftToServant(Program.I().puzzleMode);
    }

    void onClickReplay()
    {
        Program.I().shiftToServant(Program.I().selectReplay);
    }

    void onClickSetting()
    {
        Program.I().setting.show();
    }

    void onClickSelectDeck()
    {
        Program.I().shiftToServant(Program.I().selectDeck);
    }

    void onClickDownload()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN //编译器、Windows
        Application.OpenURL("https://github.com/Unicorn369/closeup_mobile/releases/tag/0.1");
#elif UNITY_ANDROID //Android
        AndroidJavaObject jo = new AndroidJavaObject("cn.unicorn369.library.API");
        if (!File.Exists("updates/closeup_0.4.txt")) {//用于检查更新
            if (File.Exists("closeup_0.4.zip")) {//如果有则直接解压
                jo.Call("doExtractZipFile", "closeup_0.4.zip", Program.ANDROID_GAME_PATH);
            } else if (File.Exists("updates/closeup_0.3.txt")){//如果有则下载更新包
                jo.Call("doDownloadFile", "https://download.ygo2019.xyz/ygopro2-data/picture/up_closeup_0.4.zip");
            } else {//否则下载并解压，锁定目录：ANDROID_GAME_PATH
                jo.Call("doDownloadFile", "https://download.ygo2019.xyz/ygopro2-data/picture/closeup_0.4.zip");
            }
        } else {
            string[] lines = {Program.ANDROID_GAME_PATH + "updates/ver_" + Program.GAME_VERSION + ".txt",
                              Program.ANDROID_GAME_PATH + "updates/ui.txt",
                              Program.ANDROID_GAME_PATH + "updates/bgm_0.1.txt",
                              Program.ANDROID_GAME_PATH + "updates/closeup_0.4.txt"};
            Program.DeleteTxt(lines);
            showToast("已是最新，无需再次下载！");
        }
#endif
    }

    public void onCheckUpgrade()
    {
        RMSshow_yesOrNo
        (
            "onCheckUpgrade",
            InterString.Get("发现新版本，是否立即下载？"),
            new messageSystemValue { hint = "yes", value = "yes" },
            new messageSystemValue { hint = "no", value = "no" }
        );
    }

    public void showToast(string content)
    {
        RMSshow_onlyYes("showToast", InterString.Get(content), null);
    }

    public static void deleteShell()
    {
        try
        {
            if (File.Exists("commamd.shell") == true)
            {
                File.Delete("commamd.shell");
            }
        }
        catch (Exception)
        {
        }
    }

    static int lastTime = 0;
    public static void checkCommend()
    {
        if (Program.TimePassed() - lastTime > 1000)
        {
            lastTime = Program.TimePassed();
            if (Program.I().selectDeck == null)
            {
                return;
            }
            if (Program.I().selectReplay == null)
            {
                return;
            }
            if (Program.I().puzzleMode == null)
            {
                return;
            }
            if (Program.I().selectServer == null)
            {
                return;
            }
            try
            {
                if (File.Exists("commamd.shell") == false)
                {
                    File.Create("commamd.shell").Close();
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            string all = "";
            try
            {
                all = File.ReadAllText("commamd.shell",Encoding.UTF8);
                string[] mats = all.Split(" ");
                if (mats.Length > 0)
                {
                    switch (mats[0])
                    {
                        case "online":
                            if (mats.Length == 5)
                            {
                                UIHelper.iniFaces();//加载用户头像
                                Program.I().selectServer.KF_onlineGame(mats[1], mats[2], mats[3], mats[4]);
                            }
                            if (mats.Length == 6)
                            {
                                UIHelper.iniFaces();
                                Program.I().selectServer.KF_onlineGame(mats[1], mats[2], mats[3], mats[4], mats[5]);
                            }
                            break;
                        case "edit":
                            if (mats.Length == 2)
                            {
                                Program.I().selectDeck.KF_editDeck(mats[1]);//编辑卡组
                            }
                            break;
                        case "replay":
                            if (mats.Length == 2)
                            {
                                UIHelper.iniFaces();
                                Program.I().selectReplay.KF_replay(mats[1]);//编辑录像
                            }
                            break;
                        case "puzzle":
                            if (mats.Length == 2)
                            {
                                UIHelper.iniFaces();
                                Program.I().puzzleMode.KF_puzzle(mats[1]);//运行残局
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
            try
            {
                if (all != "")
                {
                    if (File.Exists("commamd.shell") == true)
                    {
                        File.WriteAllText("commamd.shell", "");
                    }
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e);
            }
        }
    }
}
