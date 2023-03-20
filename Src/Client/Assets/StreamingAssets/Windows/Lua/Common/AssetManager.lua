AssetManager = {}

local this = AssetManager

AssetManager.lp = function(name, process)
    CS.AssetManager.Instance:LoadSceneCallBack(name, process)
end

function AssetManager:GetInstance()
    return this
end

function AssetManager:Init()
    CS.AssetBundleTool.AssetBundleManager.Instance:LoadAssetBundle("main", "UI", this.lp)
end
