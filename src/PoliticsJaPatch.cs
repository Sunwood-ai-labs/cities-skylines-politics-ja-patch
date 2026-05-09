using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

internal static class PoliticsJaPatch
{
    private static readonly Dictionary<string, string> Map = new Dictionary<string, string>
    {
        {"Politics & Elections Mod", "政治と選挙MOD"},
        {"Citizens elect a parliament; coalitions shape city policies. Press Ctrl+P to open panel.", "市民が議会を選び、連立政権が都市政策を動かします。Ctrl+Pでパネルを開きます。"},
        {"Politics & Elections", "政治と選挙"},
        {"No election yet - call a snap election!", "未選挙 - 解散選挙を実施できます"},
        {"選挙はまだありません - 解散選挙を実施できます！", "未選挙 - 解散選挙を実施できます"},
        {"Phase: {0} | Day {1}/{2} of campaign", "状態: {0} | 選挙運動 {1}/{2}日目"},
        {"Phase: {0} | Day {1}/{2} of term", "状態: {0} | 任期 {1}/{2}日目"},
        {"フェーズ: {0} | 選挙運動 {1}/{2}日目", "状態: {0} | 選挙運動 {1}/{2}日目"},
        {"フェーズ: {0} | 任期 {1}/{2}日目", "状態: {0} | 任期 {1}/{2}日目"},
        {"Idle", "待機中"},
        {"OpinionPolling", "世論調査"},
        {"Voting", "投票中"},
        {"Forming", "組閣中"},
        {"Governing", "政権運営中"},
        {"Failed", "失敗"},
        {"Coalition: ", "連立: "},
        {"Coalition: (none)", "連立: なし"},
        {"Active policies:", "有効な政策:"},
        {"(none)", "なし"},
        {"Election timings (editable)", "選挙日程（編集可）"},
        {"Term length", "任期"},
        {"Campaign length", "選挙運動期間"},
        {"Re-election cooldown", "再選挙までの待機期間"},
        {"Minimize chirps", "通知を最小化"},
        {"Call snap election", "解散選挙を実施"},
        {"Voter Traits", "有権者特性"},
        {"Overlay: ", "オーバーレイ: "},
        {"Overlay: Off", "オーバーレイ: オフ"},
        {"Overlay: Party", "オーバーレイ: 政党"},
        {"オーバーレイ: Party", "オーバーレイ: 政党"},
        {"Politics: Party", "政治: 政党"},
        {"Politics: Turnout", "政治: 投票率"},
        {"Politics: Satisfaction", "政治: 満足度"},
        {"Politics info view: cycle Party / Turnout / Satisfaction", "政治情報ビュー: 政党 / 投票率 / 満足度を切替"},
        {"Party", "政党"},
        {"Turnout", "投票率"},
        {"Satisfaction", "満足度"},
        {"No data", "データなし"},
        {"Unhappy", "不満"},
        {"Happy", "満足"},
        {"Politics: Off", "政治: オフ"},
        {"High turnout", "高い投票率"},
        {"Low turnout", "低い投票率"},
        {"New Party ", "新党 "},
        {"Added party: ", "政党を追加: "},
        {"Removed party: ", "政党を削除: "},
        {"– Remove", "削除"},
        {"Drag to move", "ドラッグで移動"},
        {"Reset to defaults", "既定値に戻す"},
        {"Life conditions", "生活環境"},
        {"Lives in pollution", "汚染地域に居住"},
        {"Incumbency", "現職効果"},
        {"Incumbency bonus", "現職ボーナス"},
        {"Deficit multiplier", "赤字倍率"},
        {"Deficit sensitivity", "赤字感度"},
        {"Neutral: left alone when elected", "中立: 当選しても変更しない"},
        {"Support: will be enacted when elected", "支持: 当選時に有効化"},
        {"Oppose: will be repealed when elected", "反対: 当選時に廃止"},
        {"\nNeutral: left alone when elected", "\n中立: 当選しても変更しない"},
        {"\nSupport: will be enacted when elected", "\n支持: 当選時に有効化"},
        {"\nOppose: will be repealed when elected", "\n反対: 当選時に廃止"},
        {"  {0,-25} {1,3} seats  ({2:P1})", "  {0,-25} {1,3} 議席  ({2:P1})"},
        {"=== Election ", "=== 選挙 "},
        {" votes sampled  •  Turnout ", " 票を抽出  •  投票率 "},
        {"Voter Traits - Economic Axis Bias (-1 left ... +1 right)", "有権者特性 - 経済軸バイアス (-1 左派 ... +1 右派)"},
        {"How strongly a sustained budget deficit pushes voters toward\nthe economic right.\n  0 = feature off, deficits don't move voters.\n  1 = default curve: up to +0.35 right-wing nudge after\n      about 6 consecutive deficit weeks.\n  2 = twice as sensitive. 3 = maximum.\nApplies on top of per-voter trait biases.", "継続的な財政赤字が有権者をどれだけ経済右派へ動かすか。\n  0 = 無効。赤字は投票傾向に影響しません。\n  1 = 既定。約6週連続の赤字で最大 +0.35 右派寄り。\n  2 = 感度2倍。3 = 最大。\n有権者ごとの特性バイアスに加算されます。"},
        {"In-game days parliament waits in limbo after a FAILED coalition\nbefore auto-calling a snap re-election.\n\nOnly triggers when no combination of parties can form a majority\nwithin the coalition-partner cap. Set to 0 for immediate retries,\nhigher for a longer political deadlock. Has no effect on normal\nelections where a coalition succeeds.", "連立形成に失敗したあと、自動で解散再選挙を行うまで議会が待機するゲーム内日数。\n\n連立相手数の上限内で過半数を組めない場合だけ発動します。0なら即再選挙、数値を上げるほど政治的停滞が長くなります。連立が成立する通常選挙には影響しません。"},
        {"Nudges voters on the economic axis. Negative = left-leaning (higher\ntaxes, stronger services). Positive = right-leaning (lower taxes,\nbusiness-friendly). Only Young, Adult, and Senior citizens vote.", "有権者を経済軸上で動かします。負の値 = 左派寄り（高税率・強い公共サービス）。正の値 = 右派寄り（低税率・企業寄り）。投票するのは若年層・成人・高齢者のみです。"},
        {"Only post essential chirps: campaign start, election results, and bill passages.", "重要な通知だけ投稿します: 選挙運動開始、選挙結果、法案可決。"},
        {"Parliament size scales with your population:\n1 seat per ", "議会の規模は人口に応じて変化します:\n"},
        {"Probability a HAPPY voter rewards the sitting coalition with\ntheir vote instead of following ideology / grievances.\nOnly applies to voters whose happiness is 60 or higher.\n\n  0.00 = incumbency has no effect; voters always pick on fit.\n  0.10 = default. 1 in 10 happy voters flip to the gov.\n  0.25 = quarter of happy voters auto-renew the gov.\n  0.50 = half of happy voters auto-renew the gov; strong\n         advantage for whoever's in power while the city is\n         thriving.\n\nTip: set low (<=0.05) if you want every election to feel like\nan open contest, or high (>=0.25) to simulate hard-to-beat\nincumbents.", "幸福度の高い有権者が、思想や不満よりも現政権への評価で投票する確率。\n幸福度60以上の有権者にのみ適用されます。\n\n  0.00 = 現職効果なし。有権者は常に適合度で選びます。\n  0.10 = 既定。幸福な有権者の10人に1人が与党へ流れます。\n  0.25 = 4人に1人が現政権を更新します。\n  0.50 = 半数が現政権を支持。都市が好調なとき、与党に強い優位が出ます。\n\n接戦にしたいなら低め（<=0.05）、強い現職優位を再現したいなら高め（>=0.25）にします。"},
        {"My taxes are too high. Time for a change. ", "税金が高すぎる。変化が必要だ。"},
        {"Baissez les taxes tabarnak, on est plus capable.", "税金を下げてくれ。もう限界だ。"},
        {"Liberez-nous des liberaux tabarnak!!! ", "自由派政治から解放してくれ！"},
        {"Osti que ca me tanne, toujours plus d'impots. ", "また増税か。もううんざりだ。"},
        {"Ben voyons donc, j'en paye-tu assez d'impots?! ", "いったいどれだけ税金を払えばいいんだ？"},
        {"Maudites taxes, on a rien pantoute pour notre argent ", "税金ばかりで、見返りが何もない。"},
        {"Calisse, arretez de depenser mon argent. ", "私たちの金を使い込むのはやめてくれ。"},
        {"This government can't balance a budget. Voting right next time.", "この政権は予算の均衡も取れない。次は右派に投票する。"},
        {"We need lower taxes and more jobs. ", "減税と雇用拡大が必要だ。"},
        {"Running a city is like running a business! cut the waste.", "都市運営は事業運営と同じだ。無駄を削れ。"},
        {"Another deficit? I'm done funding this. ", "また赤字？もうこれ以上負担したくない。"},
        {"Big spenders are bankrupting us. Switching my vote.", "浪費家たちが街を破産させている。投票先を変える。"},
        {"If my household ran like this, we'd be homeless.", "家計がこんな運営だったら、もう住む家もない。"},
        {"Austerity now! ", "今こそ緊縮を！"},
        {"Finally! This was long overdue. #Win", "やっとだ！ずっと必要だった。#勝利"},
        {"Great move by parliament today. Makes a real difference.", "今日の議会は良い判断をした。本当に違いが出る。"},
        {"I actually feel represented for once.", "初めて自分の声が届いた気がする。"},
        {"Common sense wins. Well done.", "常識が勝った。よくやった。"},
        {"This is why I voted. Keep it going.", "だから投票したんだ。この調子で頼む。"},
        {"Seeing real change in my neighborhood.", "近所で本当の変化を感じている。"},
        {"Waste of money. They don't listen to us.", "税金の無駄だ。彼らは市民の声を聞いていない。"},
        {"So much for campaign promises. I'm done.", "選挙公約なんてこんなものか。もううんざりだ。"},
        {"Wrong priorities. Again.", "優先順位が間違っている。まただ。"},
        {"Wait until next election. We remember.", "次の選挙まで待っていろ。私たちは覚えている。"},
        {"Taxpayer funded nonsense.", "納税者負担のばかげた政策だ。"},
        {"This bill helps no one I know.", "この法案は私の知る誰の助けにもならない。"},
        {"Parliament votes {0}-{1} to {2} bill C-{3}: An Act to {4}.", "議会は {0}-{1} で法案 C-{3}「{4}」を{2}。"},
        {"Parliament votes {0}-{1} to pass bill C-{2} (Budget & Tax): An Act to {3}.", "議会は {0}-{1} で予算・税制法案 C-{2}「{3}」を可決。"},
        {"Parliament votes {0}-{1} to REPEAL bill C-{2}: An Act to end {3}.", "議会は {0}-{1} で法案 C-{2}「{3} の終了」を廃止。"},
        {"Adopting pre-existing policy as coalition bill: ", "既存政策を連立法案として採用: "},
        {"Policy renewed silently: ", "政策を静かに更新: "},
        {"Queued tax ", "税率変更を予約 "},
        {"Queued budget ", "予算変更を予約 "},
        {"No saved politics data; starting fresh.", "保存済みの政治データはありません。新規状態で開始します。"},
        {"Enable debug logging", "デバッグログを有効化"},
        {"Panel toggle hotkey", "パネル切替ホットキー"},
        {"Hotkey", "ホットキー"},
        {"Require Ctrl modifier", "Ctrlキーを必要にする"},
        {"Utilities", "ユーティリティ"},
        {"Open Elections panel", "選挙パネルを開く"},
        {"Election Stats", "選挙統計"},
        {"No election data yet.", "選挙データはまだありません。"},
        {"No election stat data found.", "選挙統計データが見つかりません。"},
        {"Election ", "選挙 "},
        {" votes sampled  \"  Turnout ", " 票を抽出  /  投票率 "},
        {"Vote by age group", "年齢層別の投票"},
        {"Young", "若年層"},
        {"Adult", "成人"},
        {"Senior", "高齢者"},
        {"Vote by education", "教育水準別の投票"},
        {"Uneducated", "未教育"},
        {"Educated", "教育あり"},
        {"Well-educated", "高教育"},
        {"Highly-educated", "最高教育"},
        {"Vote by wealth", "所得層別の投票"},
        {"Low wealth", "低所得"},
        {"Medium wealth", "中所得"},
        {"High wealth", "高所得"},
        {"Party colors", "政党カラー"},
        {"Why people voted", "投票理由"},
        {"Pure ideology", "純粋な思想"},
        {"High taxes", "高い税金"},
        {"Poor health", "医療不安"},
        {"High crime", "高い犯罪率"},
        {"Poor education", "教育不足"},
        {"Unemployment", "失業"},
        {"Pollution", "汚染"},
        {"Low land value", "低い地価"},
        {"Noise / trash", "騒音 / ごみ"},
        {" votes", " 票"},
        {"Opinion Polling", "世論調査"},
        {"No polling data yet - samples are collected each in-game day.", "世論調査データはまだありません。ゲーム内で1日ごとにサンプルが集計されます。"},
        {"Daily opinion poll - sample size {0} - showing last {1} day(s)", "日次世論調査 - サンプル数 {0} - 直近 {1} 日を表示"},
        {"today", "今日"},
        {"Manage Parties", "政党管理"},
        {"+ Add party", "+ 政党を追加"},
        {"Remove", "削除"},
        {"Short name", "略称"},
        {"Full name", "正式名称"},
        {"Color", "色"},
        {"Ideology (-1 ... +1)", "思想 (-1 ... +1)"},
        {"Economic (left→right)", "経済 (左派→右派)"},
        {"Social (prog→trad)", "社会 (進歩→伝統)"},
        {"Governance (lib→auth)", "統治 (自由→権威)"},
        {"Governance (lib↔auth)", "統治 (自由↔権威)"},
        {"Policies  (click to cycle: neutral -> support -> oppose)", "政策 (クリックで切替: 中立 -> 支持 -> 反対)"},
        {"Tax deltas (pct points, -10 .. +10)", "税率変化 (ポイント, -10 .. +10)"},
        {"Residential", "住宅"},
        {"Commercial", "商業"},
        {"Industrial", "産業"},
        {"Office", "オフィス"},
        {"Budget deltas (pct points, -30 .. +30)", "予算変化 (ポイント, -30 .. +30)"},
        {"Electricity", "電力"},
        {"Water", "水道"},
        {"Garbage", "ごみ"},
        {"Healthcare", "医療"},
        {"Fire", "消防"},
        {"Police", "警察"},
        {"Education", "教育"},
        {"Transport", "交通"},
        {"Beautification", "景観"},
        {"Roads", "道路"},
        {"Industry", "産業"},
        {"All", "すべて"},
        {"Green Progressive", "緑の進歩党"},
        {"Labour & Unions", "労働・組合党"},
        {"Liberal Centre", "自由中道党"},
        {"Conservative", "保守党"},
        {"Populist Movement", "市民運動党"},
        {"Democratic Socialists", "民主社会党"},
        {"Campaign begins - elections in ", "選挙運動開始 - 投票まであと "},
        {" days", " 日"},
        {"Snap election called! Campaign runs for ", "解散選挙が告示されました。選挙運動期間は "},
        {" days.", " 日です。"},
        {"Campaign season begins! Elections in ", "選挙運動期間が始まりました。投票まであと "},
        {"City News", "都市ニュース"},
        {"No voters sampled - aborting election.", "投票者サンプルがないため、選挙を中止します。"},
        {" forms government with ", " が政権を樹立。連立相手は "},
        {" partner(s).", " 党。"},
        {" wins. Coalition formed with ", " が勝利。連立相手は "},
        {" partner(s). Turnout ", " 党。投票率 "},
        {"No coalition could be formed. Snap re-election in ", "連立形成に失敗。再選挙まであと "},
        {"Coalition talks collapsed. Snap re-election in ", "連立協議が決裂。再選挙まであと "},
        {"Repealed policy: ", "廃止された政策: "},
        {"Repealed (opposed) policy: ", "反対により廃止された政策: "},
        {"Queued policy enable: ", "有効化予定の政策: "},
        {"Applied coalition policies. Count=", "連立政策を適用しました。件数="},
        {"Residential tax", "住宅税"},
        {"Commercial tax", "商業税"},
        {"Industrial tax", "産業税"},
        {"Office tax", "オフィス税"},
        {"Education budget", "教育予算"},
        {"Healthcare budget", "医療予算"},
        {"Police budget", "警察予算"},
        {"Fire budget", "消防予算"},
        {"Electricity budget", "電力予算"},
        {"Water budget", "水道予算"},
        {"Garbage budget", "ごみ予算"},
        {"Public Transport budget", "公共交通予算"},
        {"Beautification budget", "景観予算"},
        {"Roads budget", "道路予算"},
        {"Industry budget", "産業予算"},
        {"cut ", "削減 "},
        {"raise ", "引き上げ "},
        {"FreeTransport", "無料公共交通"},
        {"Recycling", "リサイクル"},
        {"Smoke Detectors", "煙感知器"},
        {"EducationBoost", "教育強化"},
        {"ExtraInsulation", "追加断熱"},
        {"BigBusiness", "大企業優遇"},
        {"HighTechHousing", "ハイテク住宅"},
        {"DoubleTime", "倍額賃金"},
        {"NoHeavy", "大型車両禁止"},
        {"OnlyAtNight", "夜間限定"},
        {"OldTown", "旧市街保護"},
        {"HeavyTrafficBan", "大型交通禁止"},
        {"provide Free Public Transport", "公共交通を無料化する"},
        {"mandate City-Wide Recycling", "市全域でリサイクルを義務化する"},
        {"require Smoke Detectors", "煙感知器を義務化する"},
        {"fund an Education Boost", "教育強化に予算をつける"},
        {"subsidize Home Insulation", "住宅断熱を補助する"},
        {"support Big Business Benefits", "大企業優遇を支援する"},
        {"incentivize High-Tech Housing", "ハイテク住宅を促進する"},
        {"enforce Double Time wages", "倍額賃金を実施する"},
        {"ban Heavy Traffic downtown", "中心部の大型交通を禁止する"},
        {"permit Night-Only Operations", "夜間限定営業を許可する"},
        {"protect the Old Town Heritage", "旧市街の遺産を保護する"},
        {"ban Heavy Traffic citywide", "市全域で大型交通を禁止する"},
        {"Concerned Citizen", "心配する市民"},
        {"Turnout: ", "投票率: "},
        {"A greener, fairer city for all.", "すべての人に、より緑で公正な都市を。"},
        {"Healthcare and housing are human rights.", "医療と住宅は人権です。"},
        {"Good jobs. Strong unions. Proud neighborhoods.", "よい仕事、強い組合、誇れる地域を。"},
        {"Smart, pragmatic progress.", "賢く現実的な前進を。"},
        {"Steady hands. Sound judgment.", "安定した手腕と健全な判断を。"},
        {"Law, order, and lower taxes.", "法と秩序、そして減税を。"},
        {"Back to basics. Back to greatness.", "基本へ戻り、偉大さを取り戻す。"},
        {"We will govern for every citizen.", "すべての市民のために統治します。"}
    };

    private static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: PoliticsJaPatch <assembly>");
            return 2;
        }

        var path = args[0];
        var assembly = AssemblyFactory.GetAssembly(path);
        var module = assembly.MainModule;
        var changed = 0;

        foreach (TypeDefinition type in module.Types)
        {
            changed += PatchType(type);
            changed += PatchEnum(type);
        }

        var partyTranslator = EnsurePartyTranslator(module);

        foreach (TypeDefinition type in module.Types)
        {
            changed += PatchPartyNameLoads(type, partyTranslator);
            changed += PatchPartyNameStores(type, partyTranslator);
        }

        AssemblyFactory.SaveAssembly(assembly, path);
        Console.WriteLine("Patched strings: " + changed);
        return 0;
    }

    private static int PatchType(TypeDefinition type)
    {
        var changed = 0;
        foreach (MethodDefinition method in type.Methods)
        {
            if (method.Name == "CodexJaPartyName") continue;
            if (!method.HasBody) continue;
            foreach (Instruction instruction in method.Body.Instructions)
            {
                if (instruction.OpCode == OpCodes.Ldstr && instruction.Operand is string)
                {
                    var value = (string)instruction.Operand;
                    string translated;
                    if (Map.TryGetValue(value, out translated))
                    {
                        instruction.Operand = translated;
                        changed++;
                    }
                }
            }
        }

        foreach (TypeDefinition nested in type.NestedTypes)
            changed += PatchType(nested);

        return changed;
    }

    private static MethodDefinition EnsurePartyTranslator(ModuleDefinition module)
    {
        TypeDefinition owner = null;
        foreach (TypeDefinition type in module.Types)
        {
            if (type.FullName == "PoliticsMod.PoliticsUserMod")
            {
                owner = type;
                break;
            }
        }

        if (owner == null)
            owner = module.Types[0];

        foreach (MethodDefinition method in owner.Methods)
        {
            if (method.Name == "CodexJaPartyName")
            {
                method.Body.Instructions.Clear();
                WritePartyTranslatorBody(module, method);
                return method;
            }
        }

        var methodDef = new MethodDefinition(
            "CodexJaPartyName",
            MethodAttributes.Public | MethodAttributes.Static,
            module.Import(typeof(string)));
        methodDef.Parameters.Add(new ParameterDefinition(module.Import(typeof(string))));
        WritePartyTranslatorBody(module, methodDef);
        owner.Methods.Add(methodDef);
        return methodDef;
    }

    private static void WritePartyTranslatorBody(ModuleDefinition module, MethodDefinition methodDef)
    {
        var equals = module.Import(typeof(string).GetMethod("Equals", new Type[] { typeof(string), typeof(string) }));
        var worker = methodDef.Body.CilWorker;
        AddStringReturn(worker, equals, "Green Progressive", "緑の進歩党");
        AddStringReturn(worker, equals, "Labour & Unions", "労働・組合党");
        AddStringReturn(worker, equals, "Liberal Centre", "自由中道党");
        AddStringReturn(worker, equals, "Conservative", "保守党");
        AddStringReturn(worker, equals, "Populist Movement", "市民運動党");
        AddStringReturn(worker, equals, "Democratic Socialists", "民主社会党");
        AddStringReturn(worker, equals, "GRN", "緑");
        AddStringReturn(worker, equals, "LAB", "労");
        AddStringReturn(worker, equals, "LIB", "中");
        AddStringReturn(worker, equals, "CON", "保");
        AddStringReturn(worker, equals, "POP", "民");
        AddStringReturn(worker, equals, "SOC", "社");
        worker.Append(worker.Create(OpCodes.Ldarg_0));
        worker.Append(worker.Create(OpCodes.Ret));
    }

    private static void AddStringReturn(CilWorker worker, MethodReference equals, string from, string to)
    {
        var next = worker.Create(OpCodes.Nop);
        worker.Append(worker.Create(OpCodes.Ldarg_0));
        worker.Append(worker.Create(OpCodes.Ldstr, from));
        worker.Append(worker.Create(OpCodes.Call, equals));
        worker.Append(worker.Create(OpCodes.Brfalse_S, next));
        worker.Append(worker.Create(OpCodes.Ldstr, to));
        worker.Append(worker.Create(OpCodes.Ret));
        worker.Append(next);
    }

    private static int PatchPartyNameLoads(TypeDefinition type, MethodDefinition translator)
    {
        var changed = 0;
        var displayType = type.FullName == "PoliticsMod.PoliticsOverlay"
            || type.FullName == "PoliticsMod.PoliticsPanel"
            || type.FullName == "PoliticsMod.ElectionStatsPanel"
            || type.FullName == "PoliticsMod.OpinionPollingPanel"
            || type.FullName == "PoliticsMod.PartyLegendRow"
            || type.FullName == "PoliticsMod.PartyEditorPanel";

        if (displayType)
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (!method.HasBody) continue;
                var instructions = method.Body.Instructions;
                for (var i = 0; i < instructions.Count; i++)
                {
                    var field = instructions[i].Operand as FieldReference;
                    if (field == null) continue;
                    if (field.DeclaringType.FullName == "PoliticsMod.PartyDef"
                        && (field.Name == "FullName" || field.Name == "ShortName"))
                    {
                        if (i + 1 < instructions.Count
                            && instructions[i + 1].Operand != null
                            && instructions[i + 1].Operand.ToString().IndexOf("CodexJaPartyName") >= 0)
                            continue;
                        method.Body.CilWorker.InsertAfter(instructions[i], method.Body.CilWorker.Create(OpCodes.Call, translator));
                        changed++;
                        i++;
                    }
                }
            }
        }

        foreach (TypeDefinition nested in type.NestedTypes)
            changed += PatchPartyNameLoads(nested, translator);

        return changed;
    }

    private static int PatchPartyNameStores(TypeDefinition type, MethodDefinition translator)
    {
        var changed = 0;
        if (type.FullName == "PoliticsMod.PartyBlob")
        {
            foreach (MethodDefinition method in type.Methods)
            {
                if (!method.HasBody || method.Name != "ToParty") continue;
                var instructions = method.Body.Instructions;
                for (var i = 0; i < instructions.Count; i++)
                {
                    var field = instructions[i].Operand as FieldReference;
                    if (field == null) continue;
                    if (instructions[i].OpCode == OpCodes.Stfld
                        && field.DeclaringType.FullName == "PoliticsMod.PartyDef"
                        && (field.Name == "FullName" || field.Name == "ShortName"))
                    {
                        if (i > 0
                            && instructions[i - 1].Operand != null
                            && instructions[i - 1].Operand.ToString().IndexOf("CodexJaPartyName") >= 0)
                            continue;
                        method.Body.CilWorker.InsertBefore(instructions[i], method.Body.CilWorker.Create(OpCodes.Call, translator));
                        changed++;
                        i++;
                    }
                }
            }
        }

        foreach (TypeDefinition nested in type.NestedTypes)
            changed += PatchPartyNameStores(nested, translator);

        return changed;
    }

    private static int PatchEnum(TypeDefinition type)
    {
        var changed = 0;
        if (type.FullName == "PoliticsMod.ElectionPhase")
        {
            changed += RenameField(type, "Idle", "待機中");
            changed += RenameField(type, "Campaign", "選挙運動");
            changed += RenameField(type, "Voting", "投票中");
            changed += RenameField(type, "Forming", "組閣中");
            changed += RenameField(type, "Governing", "政権運営中");
            changed += RenameField(type, "Failed", "失敗");
        }
        else if (type.FullName == "PoliticsMod.OverlayMode")
        {
            changed += RenameField(type, "Off", "オフ");
            changed += RenameField(type, "Party", "政党");
            changed += RenameField(type, "Turnout", "投票率");
            changed += RenameField(type, "Satisfaction", "満足度");
        }
        else if (type.FullName == "PoliticsMod.PolicyStance")
        {
            changed += RenameField(type, "Neutral", "中立");
            changed += RenameField(type, "Support", "支持");
            changed += RenameField(type, "Oppose", "反対");
        }

        foreach (TypeDefinition nested in type.NestedTypes)
            changed += PatchEnum(nested);

        return changed;
    }

    private static int RenameField(TypeDefinition type, string from, string to)
    {
        foreach (FieldDefinition field in type.Fields)
        {
            if (field.Name == from)
            {
                field.Name = to;
                return 1;
            }
        }

        return 0;
    }
}
