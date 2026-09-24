// 使用项目已有 TypeScript 编译器验证新旧 ABP 数据适配，无需额外测试依赖。
const assert = require('assert');
const fs = require('fs');
const vm = require('vm');
const ts = require('typescript');
const path = require('path');
const filename = path.join(__dirname, '../src/lib/application-configuration.ts');
const source = ts.transpileModule(fs.readFileSync(filename, 'utf8'), {
    compilerOptions: { module: ts.ModuleKind.CommonJS, target: ts.ScriptTarget.ES2017 }
}).outputText;
const context = { exports: {} };
vm.runInNewContext(source, context);
const adapt = context.exports.adaptApplicationConfiguration;
const fixture = {
    currentUser: { isAuthenticated: false, id: null },
    currentTenant: { isAvailable: false, id: null },
    auth: { grantedPolicies: { Allowed: true, Denied: false } },
    localization: {
        defaultResourceName: 'AbpPro',
        values: { AbpPro: { AppName: '档案库' } },
        languages: [{ cultureName: 'zh-Hans', displayName: '简体中文', flagIcon: 'cn' }],
        currentCulture: { name: 'zh-Hans', displayName: '中文' }
    },
    features: { values: { 'Feature.Enabled': 'true' } },
    setting: { values: { Theme: 'light' } },
    multiTenancy: { isEnabled: true }
};
const anonymous = adapt(fixture);
assert.strictEqual(anonymous.session.user, null);
assert.strictEqual(anonymous.abp.session.userId, null);
assert.strictEqual(anonymous.session.tenant, null);
assert.strictEqual(anonymous.abp.auth.grantedPermissions.Allowed, true);
assert.strictEqual(anonymous.abp.auth.grantedPermissions.Denied, undefined);
assert.strictEqual(anonymous.abp.localization.values.DAManage.AppName, '档案库');
assert.strictEqual(anonymous.abp.localization.languages[0].name, 'zh-Hans');
assert.strictEqual(anonymous.abp.localization.currentLanguage.name, 'zh-Hans');
assert.strictEqual(anonymous.abp.setting.values.Theme, 'light');
const authenticated = adapt(Object.assign({}, fixture, {
    currentUser: { isAuthenticated: true, id: 'user-guid', userName: 'tester', email: 'test@example.com' },
    currentTenant: { isAvailable: true, id: 'tenant-guid', name: '测试租户' }
}));
assert.strictEqual(authenticated.abp.session.userId, 'user-guid');
assert.strictEqual(authenticated.session.user.emailAddress, 'test@example.com');
assert.strictEqual(authenticated.session.tenant.tenancyName, '测试租户');
assert.strictEqual(authenticated.abp.session.tenantId, 'tenant-guid');
assert.strictEqual(authenticated.session.application.features.SignalR, undefined);
assert.throws(() => adapt({ result: fixture }), /应用配置/);
console.log('PASS: 匿名/登录会话、租户、权限拒绝、语言资源及错误响应适配');
