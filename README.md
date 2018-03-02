# SnippetManager
csファイルからスニペットファイルを自動生成するツールです。  
VisualStudioデフォルトのCodeSnippetと、ReSharperのLiveTemplatesに対応。

## Usage
ビルドしてアプリケーションを作成し、アプリケーションファイルと同じ場所に[Settings.txt](/Settings/Settings.txt)を置いてください。  

## 1. [Settings.txt](/Settings/Settings.txt)
""で囲まれた中にPathを指定してください。絶対でも相対でも大丈夫だと思います。

* **CodeFolderPath**  
スニペットの元となるcsファイルがあるディレクトリを指定。  
サブディレクトリ全てを対象とするので注意してください。

* **VSSnippetFolderPath**  
VisualStudioデフォルトのスニペットファイルを作成するディレクトリを指定。  
```C:\Users\[User名]\Documents\Visual Studio 20[xx]\Code Snippets\Visual C#\My Code Snippets```  
にしておくと、VisualStudioが勝手にスニペットを認識するので便利です。

* **RSLiveTemplateFolderPath**  
ReSharperのLiveTemplateを作成するディレクトリを指定。 

## 2. Create Snippet
いつも通りにcsファイルにプログラムを殴り書き、以下のタグを追加するだけです。

* **//$[shortcut]**  
```//$```で始まるタグで囲まれた部分がスニペットとして認識されます。  
タグの後の文字列がショートカット名となります。  
```csharp
//$gcd
public static int Gcd(int x, int y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$gcd
```

* **//@[description]**  
```//@```で始まるタグの後にそのスニペットの概要を付け足すことが出来ます。  
このタグは前述のタグで囲まれた内部に置いてください。  
```csharp
//$gcd
//@ Greatest Common Divisor （最大公約数を求めます）
public static int Gcd(int x, int y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$gcd
```

* **\_\_[parameter]\_\_**  
```__```で囲まれた文字はスニペット貼り付け時編集可能なパラメーターになります。  
型に対してパラメーターを使用したいときは、以下のサンプルが参考になります。  
```csharp
using __int__ = Int32; //スニペットタグ外に記述。__int__をint型として扱うようにしコンパイルが通るように。
//型をパラメーターにしておくと、long, byteなど他の型用のスニペットが不要になる。

//$gcd
public static __int__ Gcd(__int__ x, __int__ y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$gcd
```

* **/\*$SELECTED$\*/**  
スニペット貼り付け時、選択していたものがこのタグの位置に挿入されます。  

* **/\*$END$\*/**  
スニペット貼り付け時のキャレット（点滅している棒）の位置になります。

```csharp
//$cww
Console.Write(/*$SELECTED$*//*$END$*/);
//$cww
```

## 3. Import Snippet
アプリケーションを実行すると指定した場所にスニペットファイルが作成されます。

* **VisualStudioデフォルトのCodeSnippet**  
ツール → コードスニペットマネージャー → インポート。  
でもできますが、前述の通りスニペットファイルの出力パスを  
```C:\Users\[User名]\Documents\Visual Studio 20[xx]\Code Snippets\Visual C#\My Code Snippets```  
にしておく方がインポート不要でスニペットを利用できるのでよいです。

* **ReSharperLiveTemplates**  
ReSharper → Tools → TemplatesExplorer → 四角形に矢印が刺さっているボタンからインポート。

## Other
ReSharperLiveTemplateですが、どの場所でもショートカットが表示されるのが鬱陶しかったので  
それっぽいところでのみ候補として表示されるようにしました。  
最初の行のインデント数でスニペットのタイプを判別しています。  
* 0 or 1: Type Or Namespace  
* 2: TypeMember  
* 3↑: Statement or Expression  
