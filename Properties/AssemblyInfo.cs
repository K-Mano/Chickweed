﻿using System.Resources;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;

// アセンブリに関する一般情報は以下の属性セットをとおして制御されます。
// アセンブリに関連付けられている情報を変更するには、
// これらの属性値を変更してください。
[assembly: AssemblyTitle("Chickweed")]
[assembly: AssemblyDescription("特定の環境下におけるネットワーク管理に必要な機能を提供します。")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("ITToolKit Team")]
[assembly: AssemblyProduct("Chickweed Powered by ITToolKit™")]
[assembly: AssemblyCopyright("Copyright © ITToolKit Team 2018-2020 All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// ComVisible を false に設定すると、このアセンブリ内の型は COM コンポーネントから
// 参照できなくなります。COM からこのアセンブリ内の型にアクセスする必要がある場合は、
// その型の ComVisible 属性を true に設定してください。
[assembly: ComVisible(false)]

//ローカライズ可能なアプリケーションのビルドを開始するには、
//.csproj ファイルの <UICulture>CultureYouAreCodingWith</UICulture> を
//<PropertyGroup> 内部で設定します。たとえば、
//ソース ファイルで英語を使用している場合、<UICulture> を en-US に設定します。次に、
//下の NeutralResourceLanguage 属性のコメントを解除します。下の行の "en-US" を
//プロジェクト ファイルの UICulture 設定と一致するよう更新します。

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //テーマ固有のリソース ディクショナリが置かれている場所
                                     //(リソースがページ、
                                     // またはアプリケーション リソース ディクショナリに見つからない場合に使用されます)
    ResourceDictionaryLocation.SourceAssembly //汎用リソース ディクショナリが置かれている場所
                                              //(リソースがページ、
                                              //アプリケーション、またはいずれのテーマ固有のリソース ディクショナリにも見つからない場合に使用されます)
)]


// アセンブリのバージョン情報は次の 4 つの値で構成されています:
//
//      メジャー バージョン
//      マイナー バージョン
//      ビルド番号
//      リビジョン
//
// すべての値を指定するか、次を使用してビルド番号とリビジョン番号を既定に設定できます
// 既定値にすることができます:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.*")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: NeutralResourcesLanguage("ja-JP")]
[assembly: Guid("02477733-F54B-49AB-A959-3F9E57B60B94")]
