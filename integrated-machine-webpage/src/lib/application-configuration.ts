/**
 * 将 humanoid-archive-wms 的应用配置转换为一体机现有 ABP/Vuex 数据结构。
 * @param configuration 新版接口直接返回的配置对象，不包含旧版 result 包装。
 * @returns ABP 全局配置与会话；未登录用户、未选择租户均为 null。
 */
export function adaptApplicationConfiguration(configuration: any) {
    if (!configuration || !configuration.currentUser || !configuration.localization || !configuration.auth) {
        throw new Error('WMS 应用配置格式不正确');
    }
    const currentUser = configuration.currentUser;
    const currentTenant = configuration.currentTenant || {};
    const localization = configuration.localization;
    const values = localization.values || {};
    const policies = configuration.auth.grantedPolicies || {};
    const grantedPermissions: { [name: string]: boolean } = {};
    // 旧 ABP 通过键是否存在判断授权，不能把值为 false 的策略写入授权集合。
    Object.keys(policies).forEach(name => {
        if (policies[name] === true) grantedPermissions[name] = true;
    });
    const languages = (localization.languages || []).map(language => ({
        name: language.cultureName,
        displayName: language.displayName,
        icon: language.flagIcon ? 'famfamfam-flags ' + language.flagIcon : '',
        isDisabled: false
    }));
    const culture = localization.currentCulture || {};
    const user = currentUser.isAuthenticated ? {
        ...currentUser,
        emailAddress: currentUser.email,
        surname: currentUser.surName
    } : null;
    const tenant = currentTenant.isAvailable ? {
        ...currentTenant,
        tenancyName: currentTenant.name
    } : null;
    return {
        abp: {
            session: { userId: user ? user.id : null, tenantId: tenant ? tenant.id : null },
            auth: { allPermissions: { ...policies }, grantedPermissions },
            localization: {
                ...localization,
                defaultSourceName: 'DAManage',
                // 旧页面仍引用 DAManage 资源名，复用新后端默认资源。
                values: { ...values, DAManage: values[localization.defaultResourceName] || {} },
                languages,
                currentLanguage: languages.find(language => language.name === culture.name) || {
                    name: culture.name, displayName: culture.displayName, icon: ''
                }
            },
            setting: configuration.setting || { values: {} },
            features: configuration.features || { values: {} },
            multiTenancy: configuration.multiTenancy || { isEnabled: false },
            clock: configuration.clock || {},
            timing: configuration.timing || {}
        },
        session: {
            user,
            tenant,
            // 新后端没有旧版 SignalR 功能标记，不启动旧版 /signalr 握手。
            application: { features: (configuration.features || {}).values || {} }
        }
    };
}
