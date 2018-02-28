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

* **//$[shortcut]** - SnippetTag  
```//$```で始まるタグで囲まれた部分がスニペットとして認識されます。  
以下のサンプルは、ショートカット名"GCD"のスニペットです。
```csharp
//$GCD
public static int Gcd(int x, int y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$GCD
```

* **//@[description]** - DescriptionTag  
```//@```で始まるタグの後にそのスニペットの概要を付け足すことが出来ます。  
このタグはスニペットタグで囲まれた内部に置いてください。  
```csharp
//$GCD
//@ Greatest Common Divisor （最大公約数を求めます）
public static int Gcd(int x, int y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$GCD
```

* **\_\_[parameter]\_\_** - Parameter  
```__```で囲まれた文字はスニペット貼り付け時編集可能なパラメーターになります。  
型に対してパラメーターを使用したいときは、以下のサンプルが参考になります。  
```csharp
using __int__ = Int32; //スニペットタグ外に記述。__int__をint型として扱うようにしコンパイルが通るように。
//型をパラメーターにしておくと、long, byteなど他の型用のスニペットが不要になる。

//$GCD
public static __int__ Gcd(__int__ x, __int__ y)
{
    return y == 0 ? x : Gcd(y, x % y);
}
//$GCD
```

## 3. Import Snippet
アプリケーションを実行すると指定した場所にスニペットファイルが作成されます。

* **VisualStudioデフォルトのCodeSnippet**  
ツール → コードスニペットマネージャーから適当にインポート。  
でもできますが、前述の通りスニペットファイルの出力パスを  
```C:\Users\[User名]\Documents\Visual Studio 20[xx]\Code Snippets\Visual C#\My Code Snippets```  
にしておく方がインポート不要でスニペットを利用できるのでよいです。

* **ReSharperLiveTemplates**  
ReSharper → Tools → TemplatesExplorer → 四角形に矢印が刺さっているボタンからインポート。

## Other
* ReSharperLiveTemplateですが、どの場所でも表示されるのが鬱陶しかったので  
それっぽいところでのみ候補として表示されるようにしました。  
```例えばクラスっぽかったらクラスが記述できる場所、メソッドっぽかったらメソッドが記述できる場所等```  
適当実装なので何か不都合がありましたら教えてください。  

## License
MIT
