AssetManager = {}

local this = AssetManager

function AssetManager:GetInstance()
    return this
end

AssetManager.lp = function(name, process)
    print(name, process)
    if process >= 1 then
        CS.GameInit.Instance:TestFunc1()
    end
end

function AssetManager:Init()
    -- CS.AssetBundleTool.AssetBundleManager.Instance:LoadAssetBundle("main", "UI", this.lp)
end
