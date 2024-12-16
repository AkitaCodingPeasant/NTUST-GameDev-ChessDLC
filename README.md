# ChessDLC

ChessDLC 是一款用 C# 開發的西洋棋擴展遊戲。

## 特色功能
- **全新遊戲規則：** 每回合不再只是移動，黑白棋盤格徹底翻新。
- **棋子職階系統：** 全新的 8 大職階，共計 22 種棋子，累積功勳點數棋子還能升變。
- **技能與被動：** 共計 40 種可使用的棋子行動。移動、普通攻擊、主動技能、被動技能、奧義。
- **生命值與功勳：** 棋子不再一碰即死，利用行動對敵人造成生命值傷害，造成傷害累積功勳。
- **狀態效果：** 共計 7 種狀態效果，施展技能善用狀態效果來扭轉戰局，或為友軍附上強化。

## 安裝與執行
### 系統需求
- **建議作業系統：** Windows 10 或更新版本 / macOS 10.14 或更新版本
- **開發環境：** Visual Studio 2019 或更新版本

### 安裝步驟
1. **Clone 專案：**
   ```bash
   https://github.com/AkitaCodingPeasant/NTUST-GameDev-ChessDLC.git
   ```
2. **開啟專案：** 使用 Visual Studio 開啟專案資料夾。
3. **執行遊戲：** 在 Visual Studio 中編譯並執行即可開始遊戲。
- **注意事項：** 如果使用zip下載，.resx會被windows封鎖，需要去檔案內容中解鎖。

## 操作說明
1. 每回合有兩次```行動點數```，每次使用```移動、普通攻擊、主動技能、奧義```都會消耗行動點數，```被動技能```則不消耗。
2. 從棋盤中選取要行動的棋子。
3. 選取棋子後行動默認為```移動```要使用其他行動可以從右方技能按鈕選擇。
4. 點擊```黃色格子```確認施法，有些技能需要施法不只一次，請詳閱右方技能說明。
5. 按下不可施法格子或右方```取消技能```按鈕可取消施法
5. 施放技能後，有些技能會進入冷卻，要等待些許回合後才可再次施放。
6. 將敵方 ```Ruler``` 擊倒便結束遊戲，獲得勝利。
7. 玩得愉快，開始你的聖杯戰爭吧。

## 開發團隊
- **專案作者：** [AkitaCodingPeasant](https://github.com/AkitaCodingPeasant)
- **聯絡方式：** booboodog7272@gmail.com

## 授權條款
本專案採用 MIT License 授權，詳情請參閱 [LICENSE](./LICENSE) 文件。
