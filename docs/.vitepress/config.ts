import { defineConfig } from "vitepress";

export default defineConfig({
  title: "Politics JA Patch",
  description: "Japanese localization patcher for the Cities: Skylines Politics & Elections mod.",
  cleanUrls: true,
  lastUpdated: true,
  themeConfig: {
    logo: "/icon.svg",
    nav: [
      { text: "Guide", link: "/guide/usage" },
      { text: "日本語", link: "/ja/" }
    ],
    sidebar: [
      {
        text: "Guide",
        items: [
          { text: "Usage", link: "/guide/usage" },
          { text: "Technical Notes", link: "/guide/technical-notes" }
        ]
      }
    ],
    socialLinks: []
  },
  locales: {
    root: {
      label: "English",
      lang: "en-US"
    },
    ja: {
      label: "日本語",
      lang: "ja-JP",
      title: "Politics 日本語化パッチ",
      description: "Cities: Skylines Politics & Elections MOD の日本語化パッチャー。",
      themeConfig: {
        nav: [
          { text: "使い方", link: "/ja/guide/usage" },
          { text: "English", link: "/" }
        ],
        sidebar: [
          {
            text: "ガイド",
            items: [
              { text: "使い方", link: "/ja/guide/usage" },
              { text: "技術メモ", link: "/ja/guide/technical-notes" }
            ]
          }
        ]
      }
    }
  }
});
