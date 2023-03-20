Login = {}

local this = AssetManager

-- 单例
function Login:GetInstance()
    return this
end

-- 初始化入口
function Login:Init()
    CS.AssetBundleTool.AssetBundleManager.Instance:LoadAssetBundle("UIs", "UILogin", this.lp)
end

-- 加载资源回调
Login.lp = function(name, process)
    CS.AssetBundleTool.AssetManager.Instance:LoadSceneCallBack(name, process)
end
