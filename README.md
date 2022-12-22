﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿*AssetManager 学校固定资产管理系统*

数据库原理实验课程大作业，2022秋季学期

NWPU，2022年12月

## 简介

学校固定资产的全生命周期管理系统

* 功能：记录固定资产记录、维护信息、报废信息，提供整体统计报表数据，提醒近期变更和未来几天的维护、报废安排。
* 安全：提供基础的数据库防注入、密码加密、数据库备份功能。
* 设计：使用WinUI 3风格界面，操作简单流畅。
* 开发：Windows Template Studio (WinUI3 + MVVM Toolkit)
* 本地化：支持多语言。
* 系统要求：Windows 10 1809 (build 17763)及以上或Windows 11，x86、x64或arm64架构。需要安装[.NET Desktop Runtime 6.0.11](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 以上以及[Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads)，注意在安装时选择对应的架构。

## 界面

登陆界面

![登陆界面](/img/login_page.png)

所有固定资产

![asset table page](/img/asset_table_page.png)

## 进度

- [x] 登陆界面
- [x] 固定资产界面：
  - [x] 连接数据库、显示数据
  - [x] 修改数据
  - [x] 删除数据
  - [x] 添加数据
  - [x] 过滤
  - [x] 搜索
- [x] 人员记录
- [ ] ~~购置记录~~
- [x] 维修记录
- [x] 报废记录
- [ ] 数据库设置
- [x] 主页显示信息
  - [ ] ~~近期报废信息~~
  - [x] 近期维修信息
- [x] 数据库备份
- [ ] 分组 (Group) 功能

## 已知问题

* 在所有固定资产中，当时为了让信息更简洁可读，没有在视图中加入供应商id、领用人id、使用人id等字段，后来导致编辑和添加数据时，相当于让供应商名、领用人名等作了主键。在其他表中，已经认识到了这个问题，故不存在这个bug；

## 已知bug

* 暗色主题无法正常显示；
* 登陆成功后不能自动导航到主页；
* 每次点开数据块的编辑，即使不做任何修改也会更新UpdateList，性能有所损失；
* 由于是全字段搜索，多个字段都有匹配的话，数据会被重复添加；
* 在主页中，右边list view为空时，即第一次拖动时，如果拖动多个item，会发生错误，只能成功转移第一个item。
