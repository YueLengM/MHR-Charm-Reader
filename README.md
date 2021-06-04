# 怪物猎人崛起 护石读取器

适用于采集卡用户的护石读取器

# 需求

### 环境

-  .Net Framework 4.8

### 采集卡

- 支持 YUY2 格式
- 支持 1080P 输出

或

- 可以被正常 OSB 捕获

# 使用

## 基本使用

#### 没有其他软件使用采集卡

1. 在输入源中选择采集卡（可使用右侧按钮预览画面）
2. 点击捕获或使用快捷键
3. 粘贴到 Excel 或其他位置

#### 采集卡已经被占用

如果你没有便携屏或通过采集卡直播，那你可能正在使用一些软件来捕获采集卡画面，此时采集卡不能被共享读取，需要再将采集卡画面转播出去。

- OBS：~~使用自带的虚拟摄像头功能~~[[#3635](https://github.com/obsproject/obs-studio/issues/3635)] / 使用 [Virtualcam](https://obsproject.com/forum/resources/obs-virtualcam.949/) 插件

## 更新技能名列表

技能名文档 (Skills.txt) 是用于结果修正的关键文件

可以在[怪物猎人崛起配装器](https://mhrise.wiki-db.com/sim/?hl=zh-hans)的**护石**页面执行下面的脚本获得

```javascript
[...document.getElementsByTagName('select')[0].getElementsByTagName('option')].slice(1,-3).map(e=>e.textContent).join('\n');
```


# 处理流程

 [OpenCvSharp](https://github.com/shimat/opencvsharp) 捕获+裁剪 --> [tesseract](https://github.com/charlesw/tesseract) OCR --> [SymSpell](https://github.com/wolfgarbe/SymSpell) 结果修正

- 孔位通过区域平均亮度识别


# 已知问题

长时间使用后可能会崩溃，可能是 OpenCvSharp 造成的内存溢出。

