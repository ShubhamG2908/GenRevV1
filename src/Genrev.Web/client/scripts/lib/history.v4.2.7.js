﻿/*!  https://github.com/devote/HTML5-History-API
 *
 * History API JavaScript Library v4.2.7
 *
 * Support: IE8+, FF3+, Opera 9+, Safari, Chrome and other
 *
 * Copyright 2011-2015, Dmitrii Pakhtinov ( spb.piksel@gmail.com )
 *
 * http://spb-piksel.ru/
 *
 * MIT license:
 *   http://www.opensource.org/licenses/mit-license.php
 *
 * Update: 2016-03-08 16:57
 */
(function (m) { if ("function" === typeof define && define.amd) { if ("undefined" !== typeof requirejs) { var w = "[history" + (new Date).getTime() + "]", j = requirejs.onError; m.toString = function () { return w }; requirejs.onError = function (m) { -1 === m.message.indexOf(w) && j.call(requirejs, m) } } define([], m) } if ("object" === typeof exports && "undefined" !== typeof module) module.exports = m(); else return m() })(function () {
    var h = !0, i = null, p = !1; function m(a, b) { var c = e.history !== q; c && (e.history = q); a.apply(q, b); c && (e.history = k) } function w() { } function j(a, b, c) {
        if (a != i && "" !== a && !b) {
            var b = j(), d = g.getElementsByTagName("base")[0];
            !c && (d && d.getAttribute("href")) && (d.href = d.href, b = j(d.href, i, h)); c = b.d; d = b.h; a = "" + a; a = /^(?:\w+\:)?\/\//.test(a) ? 0 === a.indexOf("/") ? d + a : a : d + "//" + b.g + (0 === a.indexOf("/") ? a : 0 === a.indexOf("?") ? c + a : 0 === a.indexOf("#") ? c + b.e + a : c.replace(/[^\/]+$/g, "") + a)
        } else if (a = b ? a : f.href, !o || c) a = a.replace(/^[^#]*/, "") || "#", a = f.protocol.replace(/:.*$|$/, ":") + "//" + f.host + l.basepath + a.replace(RegExp("^#[/]?(?:" + l.type + ")?"), ""); N.href = a; var a = /(?:([a-zA-Z0-9\-]+\:))?(?:\/\/(?:[^@]*@)?([^\/:\?#]+)(?::([0-9]+))?)?([^\?#]*)(?:(\?[^#]+)|\?)?(?:(#.*))?/.exec(N.href),
        b = a[2] + (a[3] ? ":" + a[3] : ""), c = a[4] || "/", d = a[5] || "", e = "#" === a[6] ? "" : a[6] || "", k = c + d + e, m = c.replace(RegExp("^" + l.basepath, "i"), l.type) + d; return { b: a[1] + "//" + b + k, h: a[1], g: b, i: a[2], k: a[3] || "", d: c, e: d, a: e, c: k, j: m, f: m + e }
    } function Z() {
        var a; try { a = e.sessionStorage, a.setItem(B + "t", "1"), a.removeItem(B + "t") } catch (b) { a = { getItem: function (a) { a = g.cookie.split(a + "="); return 1 < a.length && a.pop().split(";").shift() || "null" }, setItem: function (a) { var b = {}; if (b[f.href] = k.state) g.cookie = a + "=" + s.stringify(b) } } } try {
            r = s.parse(a.getItem(B)) ||
            {}
        } catch (c) { r = {} } t(u + "unload", function () { a.setItem(B, s.stringify(r)) }, p)
    } function C(a, b, c, d) {
        var f = 0; c || (c = { set: w }, f = 1); var g = !c.set, j = !c.get, k = { configurable: h, set: function () { g = 1 }, get: function () { j = 1 } }; try { y(a, b, k), a[b] = a[b], y(a, b, c) } catch (l) { } if (!g || !j) if (a.__defineGetter__ && (a.__defineGetter__(b, k.get), a.__defineSetter__(b, k.set), a[b] = a[b], c.get && a.__defineGetter__(b, c.get), c.set && a.__defineSetter__(b, c.set)), !g || !j) {
            if (f) return p; if (a === e) {
                try { var m = a[b]; a[b] = i } catch (o) { } if ("execScript" in e) e.execScript("Public " +
                b, "VBScript"), e.execScript("var " + b + ";", "JavaScript"); else try { y(a, b, { value: w }) } catch (r) { "onpopstate" === b && (t("popstate", c = function () { H("popstate", c, p); var b = a.onpopstate; a.onpopstate = i; setTimeout(function () { a.onpopstate = b }, 1) }, p), O = 0) } a[b] = m
            } else try { try { var n = I.create(a); y(I.getPrototypeOf(n) === a ? n : a, b, c); for (var q in a) "function" === typeof a[q] && (n[q] = a[q].bind(a)); try { d.call(n, n, a) } catch (s) { } a = n } catch (u) { y(a.constructor.prototype, b, c) } } catch (v) { return p }
        } return a
    } function $(a, b, c) {
        c = c || {}; a = a ===
        P ? f : a; c.set = c.set || function (c) { a[b] = c }; c.get = c.get || function () { return a[b] }; return c
    } function aa(a, b, c) { a in v ? v[a].push(b) : 3 < arguments.length ? t(a, b, c, arguments[3]) : t(a, b, c) } function ba(a, b, c) { var d = v[a]; if (d) for (a = d.length; a--;) { if (d[a] === b) { d.splice(a, 1); break } } else H(a, b, c) } function D(a, b) {
        var c = ("" + ("string" === typeof a ? a : a.type)).replace(/^on/, ""), d = v[c]; if (d) {
            b = "string" === typeof a ? b : a; if (b.target == i) for (var f = ["target", "currentTarget", "srcElement", "type"]; a = f.pop() ;) b = C(b, a, {
                get: "type" === a ? function () { return c } :
                function () { return e }
            }); O && (("popstate" === c ? e.onpopstate : e.onhashchange) || w).call(e, b); for (var f = 0, g = d.length; f < g; f++) d[f].call(e, b); return h
        } return ca(a, b)
    } function Q() { var a = g.createEvent ? g.createEvent("Event") : g.createEventObject(); a.initEvent ? a.initEvent("popstate", p, p) : a.type = "popstate"; a.state = k.state; D(a) } function z(a, b, c, d) { o ? A = f.href : (0 === n && (n = 2), b = j(b, 2 === n && -1 !== ("" + b).indexOf("#")), b.c !== j().c && (A = d, c ? f.replace("#" + b.f) : f.hash = b.f)); !E && a && (r[f.href] = a); F = p } function R(a) {
        var b = A; A = f.href;
        if (b) { S !== f.href && Q(); var a = a || e.event, b = j(b, h), c = j(); a.oldURL || (a.oldURL = b.b, a.newURL = c.b); b.a !== c.a && D(a) }
    } function T(a) { setTimeout(function () { t("popstate", function (a) { S = f.href; E || (a = C(a, "state", { get: function () { return k.state } })); D(a) }, p) }, 0); !o && (a !== h && "location" in k) && (U(G.hash), F && (F = p, Q())) } function da(a) {
        var a = a || e.event, b; a: { for (b = a.target || a.srcElement; b;) { if ("A" === b.nodeName) break a; b = b.parentNode } b = void 0 } var c = "defaultPrevented" in a ? a.defaultPrevented : a.returnValue === p; b && ("A" === b.nodeName &&
        !c) && (c = j(), b = j(b.getAttribute("href", 2)), c.b.split("#").shift() === b.b.split("#").shift() && b.a && (c.a !== b.a && (G.hash = b.a), U(b.a), a.preventDefault ? a.preventDefault() : a.returnValue = p))
    } function U(a) { var b = g.getElementById(a = (a || "").replace(/^#/, "")); b && (b.id === a && "A" === b.nodeName) && (a = b.getBoundingClientRect(), e.scrollTo(J.scrollLeft || 0, a.top + (J.scrollTop || 0) - (J.clientTop || 0))) } var e = ("object" === typeof window ? window : this) || {}; if (!e.history || "emulate" in e.history) return e.history; var g = e.document, J = g.documentElement,
    I = e.Object, s = e.JSON, f = e.location, q = e.history, k = q, K = q.pushState, V = q.replaceState, o = function () { var a = e.navigator.userAgent; return (-1 !== a.indexOf("Android 2.") || -1 !== a.indexOf("Android 4.0")) && -1 !== a.indexOf("Mobile Safari") && -1 === a.indexOf("Chrome") && -1 === a.indexOf("Windows Phone") ? p : !!K }(), E = "state" in q, y = I.defineProperty, G = C({}, "t") ? {} : g.createElement("a"), u = "", L = e.addEventListener ? "addEventListener" : (u = "on") && "attachEvent", W = e.removeEventListener ? "removeEventListener" : "detachEvent", X = e.dispatchEvent ?
    "dispatchEvent" : "fireEvent", t = e[L], H = e[W], ca = e[X], l = { basepath: "/", redirect: 0, type: "/", init: 0 }, B = "__historyAPI__", N = g.createElement("a"), A = f.href, S = "", O = 1, F = p, n = 0, r = {}, v = {}, x = g.title, M, ea = { onhashchange: i, onpopstate: i }, Y = {
        setup: function (a, b, c) { l.basepath = ("" + (a == i ? l.basepath : a)).replace(/(?:^|\/)[^\/]*$/, "/"); l.type = b == i ? l.type : b; l.redirect = c == i ? l.redirect : !!c }, redirect: function (a, b) {
            k.setup(b, a); b = l.basepath; if (e.top == e.self) {
                var c = j(i, p, h).c, d = f.pathname + f.search; o ? (d = d.replace(/([^\/])$/, "$1/"),
                c != b && RegExp("^" + b + "$", "i").test(d) && f.replace(c)) : d != b && (d = d.replace(/([^\/])\?/, "$1/?"), RegExp("^" + b, "i").test(d) && f.replace(b + "#" + d.replace(RegExp("^" + b, "i"), l.type) + f.hash))
            }
        }, pushState: function (a, b, c) { var d = g.title; x != i && (g.title = x); K && m(K, arguments); z(a, c); g.title = d; x = b }, replaceState: function (a, b, c) { var d = g.title; x != i && (g.title = x); delete r[f.href]; V && m(V, arguments); z(a, c, h); g.title = d; x = b }, location: { set: function (a) { 0 === n && (n = 1); e.location = a }, get: function () { 0 === n && (n = 1); return G } }, state: {
            get: function () {
                return "object" ===
                typeof r[f.href] ? s.parse(s.stringify(r[f.href])) : "undefined" !== typeof r[f.href] ? r[f.href] : i
            }
        }
    }, P = {
        assign: function (a) { !o && 0 === ("" + a).indexOf("#") ? z(i, a) : f.assign(a) }, reload: function (a) { f.reload(a) }, replace: function (a) { !o && 0 === ("" + a).indexOf("#") ? z(i, a, h) : f.replace(a) }, toString: function () { return this.href }, origin: { get: function () { return void 0 !== M ? M : !f.origin ? f.protocol + "//" + f.hostname + (f.port ? ":" + f.port : "") : f.origin }, set: function (a) { M = a } }, href: o ? i : { get: function () { return j().b } }, protocol: i, host: i, hostname: i,
        port: i, pathname: o ? i : { get: function () { return j().d } }, search: o ? i : { get: function () { return j().e } }, hash: o ? i : { set: function (a) { z(i, ("" + a).replace(/^(#|)/, "#"), p, A) }, get: function () { return j().a } }
    }; if (function () {
    var a = g.getElementsByTagName("script"), a = (a[a.length - 1] || {}).src || ""; (-1 !== a.indexOf("?") ? a.split("?").pop() : "").replace(/(\w+)(?:=([^&]*))?/g, function (a, b, c) { l[b] = (c || "").replace(/^(0|false)$/, "") }); t(u + "hashchange", R, p); var b = [P, G, ea, e, Y, k]; E && delete Y.state; for (var c = 0; c < b.length; c += 2) for (var d in b[c]) if (b[c].hasOwnProperty(d)) if ("object" !==
    typeof b[c][d]) b[c + 1][d] = b[c][d]; else { a = $(b[c], d, b[c][d]); if (!C(b[c + 1], d, a, function (a, d) { if (d === k) e.history = k = b[c + 1] = a })) return H(u + "hashchange", R, p), p; b[c + 1] === e && (v[d] = v[d.substr(2)] = []) } k.setup(); l.redirect && k.redirect(); l.init && (n = 1); !E && s && Z(); if (!o) g[L](u + "click", da, p); "complete" === g.readyState ? T(h) : (!o && j().c !== l.basepath && (F = h), t(u + "load", T, p)); return h
    }()) return k.emulate = !o, e[L] = aa, e[W] = ba, e[X] = D, k
});