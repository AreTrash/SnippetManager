# SnippetManager
csファイルからスニペットファイルを自動生成するツールです。  
VisualStudioデフォルトのCodeSnippetと、ReSharperのLiveTemplateに対応。

## Usage
ビルドしてアプリケーションを作成し、アプリケーションファイルと同じ場所に[Settings.txt](/Settings/Settings.txt)を置いてください。  

### [Settings.txt](/Settings/Settings.txt)
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

### Create Snippet
* **//$[shortcut]**  - <small>スニペットタグ</small>  
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

* **//@[description]**  - <small>ディスクリプションタグ</small>  
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
