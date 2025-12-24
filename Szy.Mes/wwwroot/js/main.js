// 全局页签数据
let tabList = [];

// 页面加载完成后初始化
document.addEventListener('DOMContentLoaded', function () {
    // 1. 一级菜单展开/收起
    initLevel1Menu();
    // 2. 二级菜单点击加载三级菜单
    initLevel2Menu();
});

/**
 * 初始化一级菜单展开/收起
 */
function initLevel1Menu() {
    const level1Menus = document.querySelectorAll('.level1-menu');
    level1Menus.forEach(menu => {
        menu.addEventListener('click', function () {
            const level2Menu = this.querySelector('.level2-menu');
            // 切换显示/隐藏
            level2Menu.style.display = level2Menu.style.display === 'block' ? 'none' : 'block';
            // 切换图标
            const icon = this.querySelector('.menu-icon');
            icon.textContent = level2Menu.style.display === 'block' ? '▼' : '▶';
        });
    });
}

/**
 * 初始化二级菜单点击事件（加载三级菜单）
 */
function initLevel2Menu() {
    const level2Items = document.querySelectorAll('.level2-item');
    level2Items.forEach(item => {
        item.addEventListener('click', function () {
            const level2Id = parseInt(this.dataset.id);
            // 获取三级菜单
            const level3Menus = window.allMenus.filter(m => m.parentId === level2Id && m.level === 3);
            renderLevel3Menu(level3Menus);
        });
    });
}

/**
 * 渲染三级菜单
 */
function renderLevel3Menu(level3Menus) {
    const level3MenuEl = document.getElementById('level3-menu');
    level3MenuEl.innerHTML = '';

    if (level3Menus.length === 0) {
        level3MenuEl.innerHTML = '<div class="empty">暂无三级菜单</div>';
        document.getElementById('level4-menu').innerHTML = '';
        return;
    }

    level3Menus.forEach(menu => {
        const menuItem = document.createElement('div');
        menuItem.className = 'menu-item';
        menuItem.textContent = menu.MenuName;

        menuItem.dataset.id = menu.Id;
        level3MenuEl.appendChild(menuItem);

        // 三级菜单点击加载四级菜单
        menuItem.addEventListener('click', function () {
            const level3Id = parseInt(this.dataset.id);
            const level4Menus = window.allMenus.filter(m => m.ParentId === level3Id && m.Level === 4);
            renderLevel4Menu(level4Menus);
        });
    });
}

/**
 * 渲染四级菜单（末级，点击打开页签）
 */
function renderLevel4Menu(level4Menus) {
    const level4MenuEl = document.getElementById('level4-menu');
    level4MenuEl.innerHTML = '';

    if (level4Menus.length === 0) {
        level4MenuEl.innerHTML = '<div class="empty">暂无四级菜单</div>';
        return;
    }

    level4Menus.forEach(menu => {
        const menuItem = document.createElement('div');
        menuItem.className = 'menu-item';
        menuItem.textContent = menu.MenuName;
        menuItem.dataset.url = menu.Url;
        menuItem.dataset.title = menu.MenuName;
        // 四级菜单点击打开页签
        menuItem.addEventListener('click', function () {
            const url = this.dataset.url;
            const title = this.dataset.title;
            addTab(title, url);
        });
        level4MenuEl.appendChild(menuItem);
    });
}

/**
 * 添加页签
 */
function addTab(title, url) {
    // 检查页签是否已存在
    const existTab = tabList.find(tab => tab.Url === url);
    if (existTab) {
        // 激活已有页签
        activateTab(existTab.Id);
        return;
    }

    // 生成唯一ID
    const tabId = 'tab_' + Date.now();
    // 添加到页签列表
    const newTab = {
        Id: tabId,
        Title: title,
        Url: url,
        Active: true
    };

    // 取消其他页签激活状态
    tabList.forEach(tab => tab.Active = false);
    tabList.push(newTab);

    // 渲染页签
    renderTabs();
    // 加载页签内容
    loadTabContent(tabId, url);
}

/**
 * 渲染页签头部
 */
function renderTabs() {
    const tabItemsEl = document.getElementById('tab-items');
    tabItemsEl.innerHTML = '';

    tabList.forEach(tab => {
        const tabItem = document.createElement('div');
        tabItem.className = 'tab-item ' + (tab.Active ? 'active' : '');
        tabItem.dataset.id = tab.Id;
        tabItem.innerHTML = `
            <span>${tab.Title}</span>
            <span class="tab-close" data-id="${tab.Id}">×</span>
        `;

        // 页签点击激活
        tabItem.addEventListener('click', function () {
            activateTab(tab.Id);
        });

        // 关闭页签
        const closeBtn = tabItem.querySelector('.tab-close');
        closeBtn.addEventListener('click', function (e) {
            e.stopPropagation(); // 阻止冒泡
            closeTab(tab.Id);
        });

        tabItemsEl.appendChild(tabItem);
    });
}

/**
 * 激活指定页签
 */
function activateTab(tabId) {
    // 更新页签激活状态
    tabList.forEach(tab => {
        tab.Active = tab.Id === tabId;
    });

    // 重新渲染页签
    renderTabs();

    // 加载对应内容
    const tab = tabList.find(t => t.Id === tabId);
    if (tab) {
        loadTabContent(tabId, tab.Url);
    }
}

/**
 * 关闭指定页签
 */
function closeTab(tabId) {
    // 找到要关闭的页签索引
    const tabIndex = tabList.findIndex(tab => tab.Id === tabId);
    if (tabIndex === -1) return;

    // 如果是激活的页签，激活前一个
    const isActive = tabList[tabIndex].Active;
    tabList.splice(tabIndex, 1);

    // 激活前一个页签
    if (isActive && tabList.length > 0) {
        const newActiveTab = tabIndex > 0 ? tabList[tabIndex - 1] : tabList[0];
        newActiveTab.Active = true;
        loadTabContent(newActiveTab.Id, newActiveTab.Url);
    }

    // 重新渲染页签
    renderTabs();

    // 如果没有页签了，清空内容
    if (tabList.length === 0) {
        document.getElementById('tab-content').innerHTML = '';
    }
}

/**
 * 加载页签内容
 */
async function loadTabContent(tabId, url) {
    const tabContentEl = document.getElementById('tab-content');
    tabContentEl.innerHTML = '<div style="text-align:center; padding:50px;">加载中...</div>';

    try {
        const res = await fetch(url);
        const html = await res.text();
        // 只激活当前页签时才渲染内容
        const currentActiveTab = tabList.find(t => t.Active);
        if (currentActiveTab && currentActiveTab.Id === tabId) {
            tabContentEl.innerHTML = html;
            // 执行内容中的脚本
            executeScriptInContent(tabContentEl);
        }
    } catch (err) {
        tabContentEl.innerHTML = `<div style="text-align:center; padding:50px; color:red;">加载失败：${err.message}</div>`;
    }
}

/**
 * 执行页签内容中的脚本
 */
function executeScriptInContent(element) {
    const scripts = element.querySelectorAll('script');
    scripts.forEach(script => {
        const newScript = document.createElement('script');
        newScript.textContent = script.textContent;
        document.body.appendChild(newScript);
        document.body.removeChild(newScript);
        script.remove();
    });
}