namespace NotEngine.Rendering;

public enum EBlendingFactor
{
    Zero = 0,
    One = 1,
    SrcColor = 768,
    OneMinusSrcColor = 769,
    SrcAlpha = 770,
    OneMinusSrcAlpha = 771,
    DstAlpha = 772,
    OneMinusDstAlpha = 773,
    DstColor = 774,
    OneMinusDstColor = 775,
    SrcAlphaSaturate = 776,
    ConstantColor = 32769,
    OneMinusConstantColor = 32770,
    ConstantAlpha = 32771,
    OneMinusConstantAlpha = 32772,
    Src1Alpha = 34185,
    Src1Color = 35065,
    OneMinusSrc1Color = 35066,
    OneMinusSrc1Alpha = 35067,
}


public enum EBlendEquationMode : uint
{
    FuncAdd = 32774,
    FuncAddExt = 32774,
    Min = 32775,
    MinExt = 32775,
    Max = 32776,
    MaxExt = 32776,
    FuncSubtract = 32778,
    FuncSubtractExt = 32778,
    FuncReverseSubtract = 32779,
    FuncReverseSubtractExt = 32779,
    AlphaMinSgix = 33568,
    AlphaMaxSgix = 33569,
}

public enum ETriangleFace : uint
{
    Front = 1028,
    Back = 1029,
    FrontAndBack = 1032,
}


public enum EDepthFunction : uint
{
    Never = 512,
    Less = 513,
    Equal = 514,
    Lequal = 515,
    Greater = 516,
    Notequal = 517,
    Gequal = 518,
    Always = 519,
}

public enum EStencilFunction : uint
{
    Never = 512,
    Less = 513,
    Equal = 514,
    Lequal = 515,
    Greater = 516,
    Notequal = 517,
    Gequal = 518,
    Always = 519,
}

public enum EStencilOp : uint
{
    Zero = 0,
    Invert = 5386,
    Keep = 7680,
    Replace = 7681,
    Incr = 7682,
    Decr = 7683,
    IncrWrap = 34055,
    DecrWrap = 34056,
}


public enum EPrimitiveType : uint
{
    Points = 0,
    Lines = 1,
    LineLoop = 2,
    LineStrip = 3,
    Triangles = 4,
    TriangleStrip = 5,
    TriangleFan = 6,
    Quads = 7,
    LinesAdjacency = 10,
    LinesAdjacencyArb = 10,
    LinesAdjacencyExt = 10,
    LineStripAdjacency = 11,
    LineStripAdjacencyArb = 11,
    LineStripAdjacencyExt = 11,
    TrianglesAdjacency = 12,
    TrianglesAdjacencyArb = 12,
    TrianglesAdjacencyExt = 12,
    TriangleStripAdjacency = 13,
    TriangleStripAdjacencyArb = 13,
    TriangleStripAdjacencyExt = 13,
    Patches = 14,
}