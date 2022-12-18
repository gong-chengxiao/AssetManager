﻿﻿﻿*AssetManager 学校固定资产管理系统*

数据库原理实验课程大作业，2022秋季学期

NWPU，2022年12月

## 简介

学校固定资产的全生命周期管理系统

* 功能：记录固定资产记录、维护信息、报废信息，提供整体统计报表数据，提醒近期变更和未来几天的维护、报废安排。
* 安全：提供基础的数据库防注入、密码加密、数据库备份功能。
* 设计：使用WinUI 3风格界面，操作简单流畅。
* 开发：使用MVVM框架。
* 本地化：支持多语言。

系统要求：Windows 10 1809 (build 17763)及以上或Windows 11，x86、x64或arm64架构。需要安装[.NET Desktop Runtime 6.0.11](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 以上以及[Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads)，注意在安装时选择对应的架构。

## 界面

登陆界面

![登陆界面](.\img\login_page.png)

所有固定资产

![asset table page](.\img\asset table page.png)

## 进度

* 登陆界面。done
* 

## 已知问题

* 暗色主题无法正常显示；
* 登陆成功后不能自动导航到主页；
