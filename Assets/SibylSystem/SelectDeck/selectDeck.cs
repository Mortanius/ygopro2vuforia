﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class selectDeck : WindowServantSP
{

    UIselectableList superScrollView = null;
    UIInput searchInput = null;
    UIDeckPanel deckPanel = null;

    GameObject gameObjectDeckCategory;
    UIPopupList decksList;

    string sort = "sortByTimeDeck";


    cardPicLoader[] quickCards = new cardPicLoader[200];

    public override void initialize()
    {
        gameObjectDeckCategory = create
        (
            Program.I().new_ui_deckCategory,
            Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height * 2f, 0)),
            new Vector3(0, 0, 0),
            false,
            Program.ui_main_2d
        );
        UIHelper.registEvent(gameObjectDeckCategory, "exit_", onDeckCategory);
        UIHelper.registEvent(gameObjectDeckCategory, "new_", onNewCategory);
        UIHelper.registEvent(gameObjectDeckCategory, "rename_", onRenameCategory);
        UIHelper.registEvent(gameObjectDeckCategory, "delete_", onDeleteCategory);
        UIHelper.registEvent(gameObjectDeckCategory, "decks", deckCategory);
        decksList = UIHelper.getByName<UIPopupList>(gameObjectDeckCategory, "decks");

        createWindow(Program.I().remaster_deckManager);
        deckPanel = gameObject.GetComponentInChildren<UIDeckPanel>();
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        UIHelper.registEvent(gameObject, "sort_", onSort);
        setSortLable();
        UIHelper.registEvent(gameObject, "edit_", onEdit);
        UIHelper.registEvent(gameObject, "new_", onNew);
        UIHelper.registEvent(gameObject, "dispose_", onDispose);
        UIHelper.registEvent(gameObject, "copy_", onCopy);
        UIHelper.registEvent(gameObject, "rename_", onRename);
        UIHelper.registEvent(gameObject, "code_", onCode);
        UIHelper.registEvent(gameObject, "category_", onDeckCategory);
        searchInput = UIHelper.getByName<UIInput>(gameObject, "search_");
        superScrollView.install();
        for (int i = 0; i < quickCards.Length; i++)
        {
            quickCards[i] = deckPanel.createCard();
            quickCards[i].relayer(i);
        }
        loadCategory();
        SetActiveFalse();

    }

    void onSearch()
    {
        printFile();
        superScrollView.toTop();
    }

    void onEdit()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        if (!isShowed)
        {
            return;
        }
        onHideCategory();
        KF_editDeck(superScrollView.selectedString);
    }

    string preString = "";

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
        if (searchInput.value != preString)
        {
            preString = searchInput.value;
            onSearch();
        }
    }

    public void KF_editDeck(string deckName)
    {
        string path = DECK_PATH + deckName + ".ydk";
        if (File.Exists(path))
        {
            Config.Set("deckInUse", deckName);
            ((DeckManager)Program.I().deckManager).shiftCondition(DeckManager.Condition.editDeck);
            Program.I().shiftToServant(Program.I().deckManager);
            ((DeckManager)Program.I().deckManager).loadDeckFromYDK(path);
            ((CardDescription)Program.I().cardDescription).setTitle(deckName);
            ((DeckManager)Program.I().deckManager).setGoodLooking();
            ((DeckManager)Program.I().deckManager).returnAction =
                () =>
                {
                    if (((DeckManager)Program.I().deckManager).deckDirty)
                    {
                    RMSshow_yesOrNoOrCancle(
                          "deckManager_returnAction"
                        , InterString.Get("要保存卡组的变更吗？")
                        , new messageSystemValue { hint = "yes", value = "yes" }
                        , new messageSystemValue { hint = "no", value = "no" }
                        , new messageSystemValue { hint = "cancle", value = "cancle" }
                        );
                    }
                    else {
                        Program.I().shiftToServant(Program.I().selectDeck);
                    }
                };
        }
    }

    public override void ES_RMS(string hashCode, List<messageSystemValue> result)
    {
        base.ES_RMS(hashCode, result);
        if (hashCode == "deckManager_returnAction")
        {
            if (result[0].value == "yes")
            {
                if (Program.I().deckManager.onSave())
                {
                    Program.I().shiftToServant(Program.I().selectDeck);
                }
            }
            if (result[0].value == "no")
            {
                Program.I().shiftToServant(Program.I().selectDeck);
            }
        }
        if (hashCode == "onNew")
        {
            try
            {
                File.Create(DECK_PATH + result[0].value + ".ydk").Close();
                RMSshow_none(InterString.Get("「[?]」创建完毕。", result[0].value));
                superScrollView.selectedString = result[0].value;
                printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
        if (hashCode == "onNewCategory")
        {
            try
            {
                Directory.CreateDirectory("deck/" + result[0].value);
                RMSshow_none(InterString.Get("「[?]」创建完毕。", result[0].value));
                if (!decksList.items.Contains(result[0].value))
                    decksList.items.Add(result[0].value);
                decksList.value = result[0].value;
                onDeckCategory();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
        if (hashCode == "onRenameCategory")
        {
            try
            {
                if (!Directory.Exists("deck/" + result[0].value))
                {
                    Directory.Move("deck/" + decksList.value, "deck/" + result[0].value);
                    RMSshow_none(InterString.Get("「[?]」重命名完毕。", result[0].value));
                    decksList.items.Remove(decksList.value);
                    decksList.items.Add(result[0].value);
                    decksList.value = result[0].value;
                    onDeckCategory();
                }
                else
                {
                    RMSshow_none(InterString.Get("「[?]」该分类已存在，重命名失败。", result[0].value));
                }
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
        if (hashCode == "onDispose")
        {
            if (result[0].value == "yes")
            {
                try
                {
                    File.Delete(DECK_PATH + superScrollView.selectedString + ".ydk");
                    RMSshow_none(InterString.Get("「[?]」删除完毕。", superScrollView.selectedString));
                    printFile();
                }
                catch (Exception)
                {
                    RMSshow_none(InterString.Get("非法删除！"));
                }
            }
        }
        if (hashCode == "onDeleteCategory")
        {
            if (result[0].value == "yes")
            {
                try
                {
                    DeleteDir("deck/" + decksList.value);
                    onDeckCategory();
                }
                catch (Exception)
                {
                    RMSshow_none(InterString.Get("非法删除！"));
                }
            }
        }
        if (hashCode == "onCopy")
        {
            try
            {
                File.Copy(DECK_PATH + superScrollView.selectedString + ".ydk", DECK_PATH + result[0].value + ".ydk");
                RMSshow_none(InterString.Get("「[?]」复制完毕。", superScrollView.selectedString));
                superScrollView.selectedString = result[0].value;
                printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
        if (hashCode == "onRename")
        {
            try
            {
                File.Move(DECK_PATH + superScrollView.selectedString + ".ydk", DECK_PATH + result[0].value + ".ydk");
                RMSshow_none(InterString.Get("「[?]」重命名完毕。", superScrollView.selectedString));
                superScrollView.selectedString = result[0].value;
                printFile();
            }
            catch (Exception)
            {
                RMSshow_none(InterString.Get("非法输入！请检查输入的文件名。"));
            }
        }
    }

    void showToast(string content)
    {
        onHideCategory();
        RMSshow_onlyYes("showToast", InterString.Get(content), null);
    }

    void onNew()
    {
        RMSshow_input("onNew", InterString.Get("请输入要创建的卡组名"), UIHelper.getTimeString());
    }

    void onDispose()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        string path = DECK_PATH + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
            RMSshow_yesOrNo(
                          "onDispose"
                        , InterString.Get("确认删除「[?]」吗？", superScrollView.selectedString)
                        , new messageSystemValue { hint = "yes", value = "yes" }
                        , new messageSystemValue { hint = "no", value = "no" }
                        );
        }
    }

    void onCopy()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        string path = DECK_PATH + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
            string newname = InterString.Get("[?]的副本", superScrollView.selectedString);
            string newnamer = newname;
            int i = 1;
            while (File.Exists(DECK_PATH + newnamer + ".ydk"))
            {
                newnamer = newname + i.ToString();
                i++;
            }
            RMSshow_input("onCopy", InterString.Get("请输入复制后的卡组名"), newnamer);
        }
    }

    void onRename()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        string path = DECK_PATH + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
            RMSshow_input("onRename", InterString.Get("新的卡组名"), superScrollView.selectedString);
        }
    }

    void onCode()
    {
        if (!superScrollView.Selected())
        {
            return;
        }
        string path = DECK_PATH + superScrollView.selectedString + ".ydk";
        if (File.Exists(path))
        {
            #if UNITY_EDITOR || UNITY_STANDALONE_WIN //编译器、Windows
                System.Diagnostics.Process.Start("notepad.exe", path);
            #elif UNITY_STANDALONE_OSX //Mac OS X
                System.Diagnostics.Process.Start("open", "-e " + path);
            #elif UNITY_STANDALONE_LINUX //Linux
                System.Diagnostics.Process.Start("gedit", path);
            #elif UNITY_ANDROID //Android
                AndroidJavaObject jo = new AndroidJavaObject("cn.unicorn369.library.API");
                jo.Call("openFile", Program.GAME_PATH + path);  // Java 代码参考: https://github.com/Unicorn369/YGO2_Android_Library
            //#elif UNITY_IPHONE //iPhone
            #endif
        }
    }

    private void setSortLable()
    {
        if (Config.Get(sort,"1") == "1")
        {
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("时间排序"));
        }
        else
        {
            UIHelper.trySetLableText(gameObject, "sort_", InterString.Get("名称排序"));
        }
    }

    private void onSort()
    {
        if (Config.Get(sort,"1") == "1")
        {
            Config.Set(sort, "0");
        }
        else
        {
            Config.Set(sort, "1");
        }
        setSortLable();
        printFile();
    }


    string deckSelected = "";
    void onSelected()
    {
        if (deckSelected == superScrollView.selectedString)
        {
            onEdit();
        }
        deckSelected = superScrollView.selectedString;
        printSelected();
    }

    private void printSelected()
    {
        GameTextureManager.clearUnloaded();
        YGOSharp.Deck deck;
        DeckManager.FromYDKtoCodedDeck(DECK_PATH + deckSelected + ".ydk", out deck);
        int mainAll = 0;
        int mainMonster = 0;
        int mainSpell = 0;
        int mainTrap = 0;
        int sideAll = 0;
        int sideMonster = 0;
        int sideSpell = 0;
        int sideTrap = 0;
        int extraAll = 0;
        int extraFusion = 0;
        int extraLink = 0;
        int extraSync = 0;
        int extraXyz = 0;
        int currentIndex = 0;

        int[] hangshu = UIHelper.get_decklieshuArray(deck.Main.Count);
        foreach (var item in deck.Main)
        {
            mainAll++;
            YGOSharp.Card c = YGOSharp.CardsManager.Get(item);
            if ((c.Type & (UInt32)CardType.Monster) > 0)
            {
                mainMonster++;
            }
            if ((c.Type & (UInt32)CardType.Spell) > 0)
            {
                mainSpell++;
            }
            if ((c.Type & (UInt32)CardType.Trap) > 0)
            {
                mainTrap++;
            }
            quickCards[currentIndex].reCode(item);
            Vector2 v = UIHelper.get_hang_lieArry(mainAll - 1, hangshu);
            quickCards[currentIndex].transform.localPosition = new Vector3
                (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, (int)v.y, hangshu[(int)v.x],10)
                ,
                161.6f - v.x * 60f
                ,
                0
                );
            if (currentIndex <= 198)
            {
                currentIndex++;
            }
        }
        foreach (var item in deck.Side)
        {
            sideAll++;
            YGOSharp.Card c = YGOSharp.CardsManager.Get(item);
            if ((c.Type & (UInt32)CardType.Monster) > 0)
            {
                sideMonster++;
            }
            if ((c.Type & (UInt32)CardType.Spell) > 0)
            {
                sideSpell++;
            }
            if ((c.Type & (UInt32)CardType.Trap) > 0)
            {
                sideTrap++;
            }
            quickCards[currentIndex].reCode(item);
            quickCards[currentIndex].transform.localPosition = new Vector3
                (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, sideAll - 1, deck.Side.Count,10)
                ,
                -181.1f
                ,
                0
                );
            if (currentIndex <= 198)
            {
                currentIndex++;
            }
        }
        foreach (var item in deck.Extra)
        {
            extraAll++;
            YGOSharp.Card c = YGOSharp.CardsManager.Get(item);
            if ((c.Type & (UInt32)CardType.Fusion) > 0)
            {
                extraFusion++;
            }
            if ((c.Type & (UInt32)CardType.Synchro) > 0)
            {
                extraSync++;
            }
            if ((c.Type & (UInt32)CardType.Xyz) > 0)
            {
                extraXyz++;
            }
            if ((c.Type & (UInt32)CardType.Link) > 0)
            {
                extraLink++;
            }
            quickCards[currentIndex].reCode(item);
            quickCards[currentIndex].transform.localPosition = new Vector3
                (
                -176.3f + UIHelper.get_left_right_indexZuo(0, 352f, extraAll - 1, deck.Extra.Count, 10)
                ,
                -99.199f
                ,
                0
                );
            if (currentIndex <= 198)
            {
                currentIndex++;
            }
        }
        while (true)
        {
            quickCards[currentIndex].clear();
            if (currentIndex <= 198)
            {
                currentIndex++;
            }
            else
            {
                break;
            }
        }
        deckPanel.leftMain.text = GameStringHelper._zhukazu + mainAll;
        deckPanel.leftExtra.text = GameStringHelper._ewaikazu + extraAll;
        deckPanel.leftSide.text = GameStringHelper._fukazu + sideAll;
        deckPanel.rightMain.text = GameStringHelper._guaishou + mainMonster + " "+ GameStringHelper._mofa + mainSpell + " " + GameStringHelper._xianjing + mainTrap;
        deckPanel.rightExtra.text = GameStringHelper._ronghe + extraFusion + " " + GameStringHelper._tongtiao + extraSync + " " + GameStringHelper._chaoliang + extraXyz + " " + GameStringHelper._lianjie + extraLink;
        deckPanel.rightSide.text = GameStringHelper._guaishou + sideMonster + " " + GameStringHelper._mofa + sideSpell + " " + GameStringHelper._xianjing + sideTrap;
    }

    public override void show()
    {
        base.show();
        printFile();
        superScrollView.toTop();
        superScrollView.selectedString = Config.Get("deckInUse", "miaowu");
        printSelected();
        Program.charge();
    }

    public override void hide()
    {
        if (isShowed)
        {
            if (superScrollView.Selected())
            {
                Config.Set("deckInUse", superScrollView.selectedString);
            }
        }
        base.hide();    
    }

    void printFile()
    {
        string deckInUse = Config.Get("deckInUse","miaowu");
        superScrollView.clear();
        FileInfo[] fileInfos = (new DirectoryInfo(DECK_PATH)).GetFiles();
        if (Config.Get(sort,"1") == "1")
        {
            Array.Sort(fileInfos, UIHelper.CompareTime);
        }
        else
        {
            Array.Sort(fileInfos, UIHelper.CompareName);
        }
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".ydk")
                {
                    if (fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4) == deckInUse)
                    {
                        if (searchInput.value == "" || Regex.Replace(fileInfos[i].Name, searchInput.value, "miaowu", RegexOptions.IgnoreCase) != fileInfos[i].Name)
                        {
                            superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                        }
                    }
                }
            }
        }
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".ydk")
                {
                    if (fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4) != deckInUse)
                    {
                        if (searchInput.value == "" || Regex.Replace(fileInfos[i].Name, searchInput.value, "miaowu", RegexOptions.IgnoreCase) != fileInfos[i].Name)
                        {
                            superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                        }
                    }
                }
            }
        }
        if (superScrollView.Selected() == false)
        {
            superScrollView.selectTop();
        }
    }

    void onClickExit()
    {
        onHideCategory();
        Program.I().shiftToServant(Program.I().menu);
    }

    bool categoryShowed = true;
    public string DECK_PATH = "deck/";

    void onHideCategory()
    {
        if (!categoryShowed)
            onDeckCategory();
    }

    void onDeckCategory()
    {
        float f = Screen.height / 700f;
        gameObjectDeckCategory.transform.localScale = new Vector3(f, f, f);
        if (categoryShowed)
        {
            categoryShowed = false;
            gameObjectDeckCategory.transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            categoryShowed = true;
            gameObjectDeckCategory.transform.position = new Vector3(0, 5, 0);
        }
    }

    void onNewCategory()
    {
        onDeckCategory();
        RMSshow_input("onNewCategory", InterString.Get("请输入卡组分类名"), UIHelper.getTimeString());
    }

    void onRenameCategory()
    {
        if (decksList.value != "[---------------]")
        {
            onDeckCategory();
            RMSshow_input("onRenameCategory", InterString.Get("请输入新的卡组分类名"), UIHelper.getTimeString());
        }
        else
        {
            showToast("默认分类，无法重命名！");
        }
    }

    void onDeleteCategory()
    {
        if (decksList.value != "[---------------]")
        {
            onDeckCategory();
            RMSshow_yesOrNo
            (
                "onDeleteCategory",
                InterString.Get("确认删除卡组分类「[?]」吗？", decksList.value),
                new messageSystemValue { hint = "yes", value = "yes" },
                new messageSystemValue { hint = "no", value = "no" }
            );
        }
        else
        {
            showToast("默认分类，无法删除！");
        }
    }

    void loadCategory()
    {
        DirectoryInfo[] directoryInfos = (new DirectoryInfo("deck/")).GetDirectories();
        for (int i = 0; i < directoryInfos.Length; i++)
        {
            decksList.items.Add(directoryInfos[i].Name);
        }
    }

    void deckCategory()
    {
        if (decksList.value != "[---------------]")
        {
            DECK_PATH = "deck/" + decksList.value + "/";
            printFile();
        } else {
            DECK_PATH = "deck/";
            printFile();
        }
    }

    void DeleteDir(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                foreach (string file in Directory.GetFileSystemEntries(path))
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                        Console.WriteLine(file);
                    }
                    else
                    {
                        DeleteDir(file);
                    }
                }
                Directory.Delete(path);
                decksList.items.Remove(decksList.value);
                decksList.value = "[---------------]";
            }
        }
        catch
        {

        }
    }

}
