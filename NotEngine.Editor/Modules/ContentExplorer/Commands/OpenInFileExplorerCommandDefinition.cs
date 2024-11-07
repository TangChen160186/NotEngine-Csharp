﻿using Gemini.Framework.Commands;

namespace NotEngine.Editor.Modules.ContentExplorer.Commands;

[CommandDefinition]
public class OpenInFileExplorerCommandDefinition : CommandDefinition
{
    public override string Name => "Open In FileExplorer";
    public override string Text => Name;
    public override string ToolTip => Name;

    public override string PathData =>
        "M14,3V5H17.59L7.76,14.83L9.17,16.24L19,6.41V10H21V3M19,19H5V5H12V3H5C3.89,3 3,3.9 3,5V19A2,2 0 0,0 5,21H19A2,2 0 0,0 21,19V12H19V19Z";

    public override string PathDataForegroundName => "OpenInFileExplorerBrush";
}

