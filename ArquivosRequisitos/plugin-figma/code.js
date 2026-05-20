// =====================================================================
//  PsicoManager — Gerador de Telas (Figma Plugin)
//  Gera os protótipos das telas do sistema com base nos requisitos
//  levantados (BPMN, entrevistas, sistema análogo e formulários).
//
//  Telas geradas:
//    Módulo Psicólogo (Desktop / Web)
//      1. Dashboard            -> RF1, RF4, RF5
//      2. Agenda Dinâmica      -> RF1, RN2 (sem duplo agendamento)
//      3. Prontuário Eletrônico-> RF2, trava de edição, RNF1/RNF2
//      4. Financeiro           -> RF4, baixa manual Pix, relatório
//    Módulo Paciente (Mobile / App)
//      5. Login / Acesso       -> privacidade, discrição
//      6. Agendamento Online   -> RF9, regra 24h, pacotes recorrentes
//      7. Diário de Emoções    -> RF8
//      8. Teleconsulta + Docs  -> RF3, RF10, RF6
// =====================================================================

// ---------- Paleta (tons calmos para saúde mental) ----------
const C = {
  primary:      '#3E8C84',
  primaryDark:  '#2C625C',
  primaryLight: '#E4F1EF',
  bg:           '#F4F7F8',
  surface:      '#FFFFFF',
  text:         '#16252B',
  muted:        '#6E8087',
  border:       '#E3EAEC',
  success:      '#3FA877',
  successBg:    '#E6F4EC',
  warning:      '#D9982F',
  warningBg:    '#FBF1DC',
  danger:       '#D86A52',
  dangerBg:     '#FBEAE5',
  accent:       '#E0A458',
  white:        '#FFFFFF'
};

// ---------- Helpers de cor ----------
function hexToRgb(hex) {
  const h = hex.replace('#', '');
  return {
    r: parseInt(h.substring(0, 2), 16) / 255,
    g: parseInt(h.substring(2, 4), 16) / 255,
    b: parseInt(h.substring(4, 6), 16) / 255
  };
}
function solid(hex, opacity) {
  return [{ type: 'SOLID', color: hexToRgb(hex), opacity: opacity == null ? 1 : opacity }];
}
function shadow(blur, y, op) {
  return [{
    type: 'DROP_SHADOW',
    color: { r: 0.09, g: 0.16, b: 0.18, a: op == null ? 0.08 : op },
    offset: { x: 0, y: y == null ? 6 : y },
    radius: blur == null ? 18 : blur,
    spread: 0, visible: true, blendMode: 'NORMAL'
  }];
}

// ---------- Fontes ----------
async function loadFonts() {
  await figma.loadFontAsync({ family: 'Inter', style: 'Regular' });
  await figma.loadFontAsync({ family: 'Inter', style: 'Medium' });
  await figma.loadFontAsync({ family: 'Inter', style: 'Semi Bold' });
  await figma.loadFontAsync({ family: 'Inter', style: 'Bold' });
}

// ---------- Primitivos ----------
function frame(name, w, h, x, y, fill) {
  const f = figma.createFrame();
  f.name = name;
  f.x = x; f.y = y;
  f.resize(w, h);
  f.fills = solid(fill || C.bg);
  f.clipsContent = true;
  return f;
}

function rect(parent, o) {
  const r = figma.createRectangle();
  parent.appendChild(r);
  r.resize(o.w, o.h);
  r.x = o.x || 0; r.y = o.y || 0;
  r.fills = solid(o.fill || C.surface, o.opacity);
  if (o.radius != null) r.cornerRadius = o.radius;
  if (o.stroke) { r.strokes = solid(o.stroke); r.strokeWeight = o.strokeW || 1; }
  if (o.shadow) r.effects = shadow(o.blur, o.shadowY, o.shadowOp);
  if (o.name) r.name = o.name;
  return r;
}

function circle(parent, o) {
  const e = figma.createEllipse();
  parent.appendChild(e);
  e.resize(o.d, o.d);
  e.x = o.x || 0; e.y = o.y || 0;
  e.fills = solid(o.fill || C.primaryLight, o.opacity);
  if (o.stroke) { e.strokes = solid(o.stroke); e.strokeWeight = o.strokeW || 1; }
  return e;
}

function text(parent, str, o) {
  o = o || {};
  const t = figma.createText();
  parent.appendChild(t);
  t.fontName = { family: 'Inter', style: o.weight || 'Regular' };
  t.fontSize = o.size || 14;
  if (o.width) { t.textAutoResize = 'HEIGHT'; t.resize(o.width, 10); }
  t.characters = str;
  t.fills = solid(o.color || C.text);
  if (o.align) t.textAlignHorizontal = o.align;
  if (o.lineHeight) t.lineHeight = { value: o.lineHeight, unit: 'PIXELS' };
  if (o.spacing != null) t.letterSpacing = { value: o.spacing, unit: 'PIXELS' };
  t.x = o.x || 0; t.y = o.y || 0;
  return t;
}

// Pílula / badge (rótulo de status)
function pill(parent, label, x, y, bg, fg) {
  const t = text(parent, label, { size: 11, weight: 'Semi Bold', color: fg });
  const padX = 10, padY = 5;
  const r = rect(parent, { x: x, y: y, w: t.width + padX * 2, h: t.height + padY * 2, fill: bg, radius: 20 });
  t.x = x + padX; t.y = y + padY;
  // garante que o texto fique acima
  parent.appendChild(t);
  return { w: t.width + padX * 2, h: t.height + padY * 2 };
}

// Botão (retângulo + texto centralizado)
function button(parent, label, x, y, w, h, o) {
  o = o || {};
  rect(parent, {
    x: x, y: y, w: w, h: h,
    fill: o.fill || C.primary, radius: o.radius == null ? 12 : o.radius,
    stroke: o.stroke, strokeW: o.stroke ? 1.5 : 0,
    shadow: o.shadow, blur: 14, shadowY: 4, shadowOp: 0.16
  });
  const t = text(parent, label, { size: o.size || 14, weight: 'Semi Bold', color: o.textColor || C.white });
  t.x = x + (w - t.width) / 2;
  t.y = y + (h - t.height) / 2;
  parent.appendChild(t);
}

// Cartão branco com sombra suave
function card(parent, x, y, w, h, radius) {
  return rect(parent, { x: x, y: y, w: w, h: h, fill: C.surface, radius: radius == null ? 16 : radius, shadow: true });
}

// "Ícone" simples desenhado com formas (sem dependência de assets)
function dot(parent, x, y, color) {
  circle(parent, { x: x, y: y, d: 8, fill: color });
}

// =====================================================================
//  SIDEBAR (módulo psicólogo)
// =====================================================================
function psychoSidebar(f, active) {
  rect(f, { x: 0, y: 0, w: 248, h: f.height, fill: C.primaryDark, radius: 0 });
  circle(f, { x: 28, y: 30, d: 34, fill: C.primary });
  text(f, 'P', { x: 38, y: 36, size: 18, weight: 'Bold', color: C.white });
  text(f, 'PsicoManager', { x: 72, y: 38, size: 17, weight: 'Bold', color: C.white });

  const items = ['Dashboard', 'Agenda', 'Prontuário', 'Financeiro', 'Teleconsulta', 'Pacientes', 'Configurações'];
  let y = 110;
  items.forEach(function (it) {
    if (it === active) {
      rect(f, { x: 16, y: y - 9, w: 216, h: 42, fill: C.white, opacity: 0.16, radius: 10 });
      rect(f, { x: 16, y: y - 9, w: 4, h: 42, fill: C.accent, radius: 2 });
    }
    dot(f, 34, y + 5, it === active ? C.white : '#9FC4BF');
    text(f, it, { x: 54, y: y, size: 14, weight: it === active ? 'Semi Bold' : 'Regular', color: it === active ? C.white : '#C7DDD9' });
    y += 52;
  });

  // Card de perfil no rodapé
  rect(f, { x: 16, y: f.height - 96, w: 216, h: 72, fill: C.white, opacity: 0.10, radius: 12 });
  circle(f, { x: 28, y: f.height - 82, d: 44, fill: C.accent });
  text(f, 'LM', { x: 39, y: f.height - 70, size: 15, weight: 'Bold', color: C.white });
  text(f, 'Lívia Mesquita', { x: 82, y: f.height - 80, size: 13, weight: 'Semi Bold', color: C.white });
  text(f, 'Psicologia Clínica', { x: 82, y: f.height - 62, size: 11, color: '#BFD8D4' });
}

function topBar(f, title, subtitle) {
  text(f, title, { x: 288, y: 36, size: 24, weight: 'Bold', color: C.text });
  if (subtitle) text(f, subtitle, { x: 288, y: 70, size: 14, color: C.muted });
  // busca
  rect(f, { x: f.width - 360, y: 34, w: 250, h: 40, fill: C.surface, radius: 10, stroke: C.border });
  circle(f, { x: f.width - 348, y: 46, d: 14, stroke: C.muted, strokeW: 1.5, fill: C.surface });
  text(f, 'Buscar paciente...', { x: f.width - 326, y: 46, size: 13, color: C.muted });
  // sino
  rect(f, { x: f.width - 92, y: 34, w: 40, h: 40, fill: C.surface, radius: 10, stroke: C.border });
  text(f, '🔔', { x: f.width - 82, y: 44, size: 16 });
}

// =====================================================================
//  TELA 1 — DASHBOARD (Psicólogo)
// =====================================================================
function screenDashboard(x, y) {
  const f = frame('01 · Psicólogo — Dashboard', 1280, 832, x, y);
  psychoSidebar(f, 'Dashboard');
  topBar(f, 'Bom dia, Lívia 👋', 'Você tem 6 sessões agendadas para hoje.');

  // Cards de métricas
  const cx = 288, cw = 220, gap = 20, ctop = 120;
  const metrics = [
    ['Sessões hoje', '6', '2 online · 4 presenciais', C.primary, C.primaryLight],
    ['Faturamento (mês)', 'R$ 8.450', '+12% vs. mês anterior', C.success, C.successBg],
    ['A receber', 'R$ 1.200', '4 cobranças pendentes', C.warning, C.warningBg],
    ['Inadimplência', 'R$ 350', '1 paciente em atraso', C.danger, C.dangerBg]
  ];
  metrics.forEach(function (m, i) {
    const mx = cx + i * (cw + gap);
    card(f, mx, ctop, cw, 110);
    rect(f, { x: mx + 20, y: ctop + 20, w: 36, h: 36, fill: m[4], radius: 10 });
    dot(f, mx + 34, ctop + 34, m[3]);
    text(f, m[0], { x: mx + 20, y: ctop + 64, size: 12, color: C.muted });
    text(f, m[1], { x: mx + 130, y: ctop + 26, size: 22, weight: 'Bold', color: C.text, align: 'RIGHT', width: 70 });
    text(f, m[2], { x: mx + 20, y: ctop + 84, size: 11, color: m[3] });
  });

  // Próximas sessões
  const sx = 288, sy = 262, sw = 600;
  card(f, sx, sy, sw, 470);
  text(f, 'Agenda de hoje', { x: sx + 24, y: sy + 22, size: 16, weight: 'Bold', color: C.text });
  text(f, 'Quarta, 20 de maio', { x: sx + 24, y: sy + 46, size: 12, color: C.muted });

  const sessions = [
    ['08:00', 'Ana Laura Morais', 'Sessão presencial', C.success, 'Confirmada'],
    ['09:00', 'Gabriel do Prado', 'Teleconsulta', C.primary, 'Confirmada'],
    ['10:00', 'Ivan Bitencourt', 'Sessão presencial', C.warning, 'Aguardando'],
    ['14:00', 'Ygor Mantovanelli', 'Teleconsulta', C.success, 'Confirmada'],
    ['15:00', 'Felipe Joshua', 'Sessão presencial', C.danger, 'Cancelou']
  ];
  let ry = sy + 80;
  sessions.forEach(function (s) {
    rect(f, { x: sx + 24, y: ry, w: sw - 48, h: 64, fill: C.bg, radius: 12 });
    rect(f, { x: sx + 24, y: ry, w: 4, h: 64, fill: s[3], radius: 2 });
    text(f, s[0], { x: sx + 44, y: ry + 22, size: 15, weight: 'Bold', color: C.text });
    text(f, s[1], { x: sx + 120, y: ry + 14, size: 14, weight: 'Semi Bold', color: C.text });
    text(f, s[2], { x: sx + 120, y: ry + 34, size: 12, color: C.muted });
    pill(f, s[4], sx + sw - 150, ry + 20, s[3] === C.danger ? C.dangerBg : C.successBg, s[3] === C.danger ? C.danger : C.success);
    ry += 74;
  });

  // Coluna lateral: alertas (mapeia Dores 3 e 5 do BPMN)
  const ax = 912, aw = 344;
  card(f, ax, sy, aw, 224);
  text(f, 'Alertas', { x: ax + 22, y: sy + 22, size: 16, weight: 'Bold', color: C.text });
  const alerts = [
    ['Falta sem aviso', 'Felipe cancelou há 30 min', C.danger, C.dangerBg],
    ['Pagamento atrasado', 'Ivan · R$ 350 (5 dias)', C.warning, C.warningBg],
    ['Prontuário pendente', '2 evoluções sem registro', C.primary, C.primaryLight]
  ];
  let ay = sy + 56;
  alerts.forEach(function (a) {
    rect(f, { x: ax + 22, y: ay, w: aw - 44, h: 48, fill: a[3], radius: 10 });
    dot(f, ax + 36, ay + 21, a[2]);
    text(f, a[0], { x: ax + 52, y: ay + 8, size: 13, weight: 'Semi Bold', color: C.text });
    text(f, a[1], { x: ax + 52, y: ay + 26, size: 11, color: C.muted });
    ay += 56;
  });

  // Atalhos
  card(f, ax, sy + 244, aw, 226);
  text(f, 'Atalhos rápidos', { x: ax + 22, y: sy + 266, size: 16, weight: 'Bold', color: C.text });
  button(f, '+ Novo agendamento', ax + 22, sy + 300, aw - 44, 44, { shadow: true });
  button(f, 'Registrar evolução', ax + 22, sy + 354, aw - 44, 44, { fill: C.primaryLight, textColor: C.primaryDark });
  button(f, 'Emitir recibo / atestado', ax + 22, sy + 408, aw - 44, 44, { fill: C.bg, textColor: C.text, stroke: C.border });
  return f;
}

// =====================================================================
//  TELA 2 — AGENDA DINÂMICA (Psicólogo)
// =====================================================================
function screenAgenda(x, y) {
  const f = frame('02 · Psicólogo — Agenda', 1280, 832, x, y);
  psychoSidebar(f, 'Agenda');
  topBar(f, 'Agenda', 'Visão semanal · 18 a 24 de maio');
  button(f, '+ Novo agendamento', f.width - 320, 36, 200, 40, { shadow: true });

  const gx = 288, gy = 120, gw = 968, gh = 612;
  card(f, gx, gy, gw, gh);

  const days = ['Seg 18', 'Ter 19', 'Qua 20', 'Qui 21', 'Sex 22'];
  const colW = (gw - 80) / days.length;
  const startTop = gy + 56;
  // cabeçalho de colunas
  text(f, 'Hora', { x: gx + 16, y: gy + 22, size: 11, weight: 'Semi Bold', color: C.muted });
  days.forEach(function (d, i) {
    const dx = gx + 80 + i * colW;
    text(f, d, { x: dx, y: gy + 20, size: 13, weight: i === 2 ? 'Bold' : 'Semi Bold', color: i === 2 ? C.primary : C.text, width: colW - 8, align: 'CENTER' });
  });
  if (true) rect(f, { x: gx + 80 + 2 * colW, y: gy + 50, w: colW, h: gh - 70, fill: C.primaryLight, opacity: 0.4, radius: 8 });

  const hours = ['08:00', '09:00', '10:00', '11:00', '12:00', '13:00', '14:00', '15:00', '16:00'];
  const rowH = (gh - 72) / hours.length;
  hours.forEach(function (h, i) {
    const hy = startTop + i * rowH;
    text(f, h, { x: gx + 16, y: hy + 6, size: 11, color: C.muted });
    rect(f, { x: gx + 70, y: hy, w: gw - 86, h: 1, fill: C.border });
  });

  // eventos: [col, hora-index, duração(linhas), título, cor, bg]
  function ev(col, hi, dur, title, who, color, bg) {
    const ex = gx + 80 + col * colW + 4;
    const ey = startTop + hi * rowH + 3;
    rect(f, { x: ex, y: ey, w: colW - 12, h: rowH * dur - 8, fill: bg, radius: 8 });
    rect(f, { x: ex, y: ey, w: 3, h: rowH * dur - 8, fill: color, radius: 2 });
    text(f, title, { x: ex + 10, y: ey + 6, size: 11, weight: 'Semi Bold', color: color, width: colW - 24 });
    text(f, who, { x: ex + 10, y: ey + 20, size: 10, color: C.muted, width: colW - 24 });
  }
  ev(0, 0, 1, '08:00 Sessão', 'Mariana', C.primary, C.primaryLight);
  ev(0, 4, 2, 'Almoço', 'Bloqueio', C.muted, C.bg);
  ev(1, 1, 1, '09:00 Teleconsulta', 'Pedro', C.success, C.successBg);
  ev(2, 0, 1, '08:00 Ana Laura', 'Presencial', C.primary, C.primaryLight);
  ev(2, 1, 1, '09:00 Gabriel', 'Online', C.success, C.successBg);
  ev(2, 6, 1, '14:00 Ygor', 'Online', C.success, C.successBg);
  ev(3, 2, 1, '10:00 Sessão', 'Beatriz', C.primary, C.primaryLight);
  ev(4, 4, 5, 'Férias', 'Bloqueio inteligente', C.accent, C.warningBg);

  // legenda + nota da regra de negócio
  text(f, 'Bloqueio inteligente ativo: almoço (12h–14h) e férias (sex). RN: o sistema impede dois agendamentos no mesmo horário.',
    { x: gx + 16, y: gy + gh - 22, size: 11, color: C.muted, width: gw - 32 });
  return f;
}

// =====================================================================
//  TELA 3 — PRONTUÁRIO ELETRÔNICO (Psicólogo)
// =====================================================================
function screenProntuario(x, y) {
  const f = frame('03 · Psicólogo — Prontuário', 1280, 832, x, y);
  psychoSidebar(f, 'Prontuário');
  topBar(f, 'Prontuário Eletrônico', 'Registro sigiloso · acesso restrito ao profissional');

  // cabeçalho do paciente
  const px = 288, pw = 968;
  card(f, px, 120, pw, 96);
  circle(f, { x: px + 24, y: 144, d: 48, fill: C.primaryLight });
  text(f, 'AL', { x: px + 38, y: 158, size: 17, weight: 'Bold', color: C.primaryDark });
  text(f, 'Ana Laura Morais', { x: px + 88, y: 140, size: 18, weight: 'Bold', color: C.text });
  text(f, '32 anos · TCC · Início: jan/2026 · 14 sessões', { x: px + 88, y: 166, size: 13, color: C.muted });
  pill(f, '🔒 Sigiloso (LGPD)', px + pw - 360, 152, C.primaryLight, C.primaryDark);
  pill(f, 'CFP nº 01/2009', px + pw - 200, 152, C.bg, C.muted);

  // editor de evolução
  const ex = 288, ew = 632, ey = 236;
  card(f, ex, ey, ew, 496);
  text(f, 'Evolução da sessão — 20/05/2026', { x: ex + 24, y: ey + 22, size: 16, weight: 'Bold', color: C.text });

  // barra de ferramentas fake
  rect(f, { x: ex + 24, y: ey + 56, w: ew - 48, h: 40, fill: C.bg, radius: 10 });
  ['B', 'I', 'U', '• Lista', 'H'].forEach(function (b, i) {
    text(f, b, { x: ex + 40 + i * 56, y: ey + 67, size: 13, weight: 'Semi Bold', color: C.muted });
  });

  rect(f, { x: ex + 24, y: ey + 108, w: ew - 48, h: 230, fill: C.surface, radius: 10, stroke: C.border });
  text(f,
    'Paciente relatou melhora no padrão de sono após adoção das estratégias de higiene do sono discutidas na sessão anterior. ' +
    'Trabalhamos reestruturação cognitiva sobre crenças de desempenho no trabalho. ' +
    'Demonstrou maior consciência dos gatilhos de ansiedade.\n\nPlano: manter registro diário de pensamentos automáticos.',
    { x: ex + 40, y: ey + 124, size: 13, color: C.text, width: ew - 80, lineHeight: 21 });

  // anexos
  text(f, 'Anexos', { x: ex + 24, y: ey + 352, size: 13, weight: 'Semi Bold', color: C.text });
  const files = ['📄 Inventário Beck.pdf', '📄 Anamnese.pdf', '+ Anexar'];
  files.forEach(function (fl, i) {
    const fx = ex + 24 + i * 150;
    rect(f, { x: fx, y: ey + 378, w: 140, h: 40, fill: i === 2 ? C.primaryLight : C.bg, radius: 10, stroke: i === 2 ? null : C.border });
    text(f, fl, { x: fx + 12, y: ey + 390, size: 11, weight: i === 2 ? 'Semi Bold' : 'Regular', color: i === 2 ? C.primaryDark : C.text });
  });

  button(f, 'Salvar evolução', ex + 24, ey + 436, 180, 44, { shadow: true });
  text(f, 'Salvo automaticamente às 15:42', { x: ex + 220, y: ey + 450, size: 12, color: C.muted });

  // coluna direita: trava + documentos + histórico
  const rx = 940, rw = 316;
  // trava de edição (requisito da Lívia)
  card(f, rx, ey, rw, 132);
  rect(f, { x: rx, y: ey, w: rw, h: 132, fill: C.warningBg, radius: 16 });
  text(f, '🔒 Trava de integridade', { x: rx + 22, y: ey + 22, size: 14, weight: 'Bold', color: C.text });
  text(f, 'Esta evolução poderá ser editada por mais 23h47min. Após esse prazo o registro é selado (RNF Integridade).',
    { x: rx + 22, y: ey + 48, size: 12, color: C.muted, width: rw - 44, lineHeight: 18 });

  // modelos de documentos
  card(f, rx, ey + 148, rw, 168);
  text(f, 'Modelos automáticos', { x: rx + 22, y: ey + 170, size: 14, weight: 'Bold', color: C.text });
  ['Recibo de pagamento', 'Atestado de comparecimento', 'Declaração / Laudo'].forEach(function (m, i) {
    rect(f, { x: rx + 22, y: ey + 200 + i * 38, w: rw - 44, h: 32, fill: C.bg, radius: 8 });
    text(f, m, { x: rx + 34, y: ey + 208 + i * 38, size: 12, color: C.text });
    text(f, 'Gerar', { x: rx + rw - 70, y: ey + 208 + i * 38, size: 12, weight: 'Semi Bold', color: C.primary });
  });

  // histórico
  card(f, rx, ey + 332, rw, 164);
  text(f, 'Histórico de sessões', { x: rx + 22, y: ey + 354, size: 14, weight: 'Bold', color: C.text });
  ['13/05 · Sessão 13', '06/05 · Sessão 12', '29/04 · Sessão 11'].forEach(function (hh, i) {
    dot(f, rx + 26, ey + 392 + i * 32, C.primary);
    text(f, hh, { x: rx + 42, y: ey + 386 + i * 32, size: 12, color: C.text });
    text(f, 'Ver', { x: rx + rw - 56, y: ey + 386 + i * 32, size: 12, weight: 'Semi Bold', color: C.muted });
  });
  return f;
}

// =====================================================================
//  TELA 4 — FINANCEIRO (Psicólogo)
// =====================================================================
function screenFinanceiro(x, y) {
  const f = frame('04 · Psicólogo — Financeiro', 1280, 832, x, y);
  psychoSidebar(f, 'Financeiro');
  topBar(f, 'Financeiro', 'Relatório mensal · Maio/2026');
  button(f, 'Exportar relatório', f.width - 320, 36, 200, 40, { fill: C.primaryLight, textColor: C.primaryDark });

  // métricas
  const cx = 288, cw = 226, gap = 20, ctop = 120;
  const metrics = [
    ['Faturamento previsto', 'R$ 9.800', C.primary],
    ['Recebido', 'R$ 8.450', C.success],
    ['A receber', 'R$ 1.000', C.warning],
    ['Inadimplência', 'R$ 350', C.danger]
  ];
  metrics.forEach(function (m, i) {
    const mx = cx + i * (cw + gap);
    card(f, mx, ctop, cw, 96);
    rect(f, { x: mx, y: ctop, w: 4, h: 96, fill: m[2], radius: 2 });
    text(f, m[0], { x: mx + 20, y: ctop + 22, size: 12, color: C.muted });
    text(f, m[1], { x: mx + 20, y: ctop + 46, size: 24, weight: 'Bold', color: C.text });
  });

  // gráfico de barras simples (faturamento por mês)
  const chx = 288, chy = 240, chw = 600, chh = 280;
  card(f, chx, chy, chw, chh);
  text(f, 'Faturamento dos últimos 6 meses', { x: chx + 24, y: chy + 22, size: 16, weight: 'Bold', color: C.text });
  const bars = [
    ['Dez', 0.55], ['Jan', 0.62], ['Fev', 0.70], ['Mar', 0.66], ['Abr', 0.78], ['Mai', 0.86]
  ];
  const baseY = chy + chh - 50, maxH = 150, bw = 48;
  bars.forEach(function (b, i) {
    const bx = chx + 50 + i * 88;
    const bh = maxH * b[1];
    rect(f, { x: bx, y: baseY - bh, w: bw, h: bh, fill: i === 5 ? C.primary : C.primaryLight, radius: 8 });
    text(f, b[0], { x: bx, y: baseY + 10, size: 12, color: C.muted, width: bw, align: 'CENTER' });
  });
  rect(f, { x: chx + 40, y: baseY, w: chw - 70, h: 1, fill: C.border });

  // lista de cobranças com baixa manual (Pix) — requisito da Lívia
  const lx = 908, lw = 348;
  card(f, lx, chy, lw, 280);
  text(f, 'Cobranças', { x: lx + 22, y: chy + 22, size: 16, weight: 'Bold', color: C.text });
  const cobr = [
    ['Ana Laura', 'R$ 250', 'Pago', C.success, C.successBg],
    ['Gabriel', 'R$ 250', 'Pix recebido', C.primary, C.primaryLight],
    ['Ivan', 'R$ 350', 'Atrasado', C.danger, C.dangerBg],
    ['Ygor', 'R$ 250', 'Pendente', C.warning, C.warningBg]
  ];
  let cy = chy + 56;
  cobr.forEach(function (c) {
    rect(f, { x: lx + 22, y: cy, w: lw - 44, h: 48, fill: C.bg, radius: 10 });
    text(f, c[0], { x: lx + 36, y: cy + 9, size: 13, weight: 'Semi Bold', color: C.text });
    text(f, c[1], { x: lx + 36, y: cy + 27, size: 11, color: C.muted });
    pill(f, c[2], lx + lw - 150, cy + 14, c[4], c[3]);
    cy += 56;
  });
  button(f, 'Dar baixa manual (Pix)', lx + 22, cy + 4, lw - 44, 40, { shadow: true });

  // nota
  text(f, 'Modelo de pagamento: paciente paga via Pix direto e o profissional dá baixa manual (sem convênios). Relatório com faturamento, inadimplência e previsão de recebimento.',
    { x: 288, y: 540, size: 12, color: C.muted, width: 600, lineHeight: 18 });
  return f;
}

// =====================================================================
//  SCAFFOLD MOBILE (módulo paciente)
// =====================================================================
function phone(name, x, y) {
  const f = frame(name, 390, 844, x, y, C.bg);
  f.cornerRadius = 40;
  // status bar
  text(f, '9:41', { x: 28, y: 18, size: 13, weight: 'Semi Bold', color: C.text });
  text(f, '5G  ', { x: 320, y: 18, size: 12, color: C.text });
  rect(f, { x: 350, y: 20, w: 22, h: 11, fill: C.text, radius: 3 });
  return f;
}
function bottomNav(f, active) {
  rect(f, { x: 0, y: f.height - 84, w: f.width, h: 84, fill: C.surface, radius: 0, shadow: true, blur: 20, shadowY: -4, shadowOp: 0.06 });
  const tabs = ['Início', 'Agenda', 'Diário', 'Perfil'];
  const tw = f.width / tabs.length;
  tabs.forEach(function (t, i) {
    const tx = i * tw;
    const on = t === active;
    circle(f, { x: tx + tw / 2 - 4, y: f.height - 60, d: 8, fill: on ? C.primary : '#C2CFD2' });
    text(f, t, { x: tx, y: f.height - 42, size: 11, weight: on ? 'Semi Bold' : 'Regular', color: on ? C.primary : C.muted, width: tw, align: 'CENTER' });
  });
}

// =====================================================================
//  TELA 5 — LOGIN / ACESSO (Paciente)
// =====================================================================
function screenLogin(x, y) {
  const f = phone('05 · Paciente — Acesso', x, y);
  // topo com gradiente simulado
  rect(f, { x: 0, y: 0, w: f.width, h: 360, fill: C.primaryDark, radius: 0 });
  rect(f, { x: 0, y: 0, w: f.width, h: 360, fill: C.primary, opacity: 0.55, radius: 0 });
  circle(f, { x: f.width / 2 - 36, y: 110, d: 72, fill: C.white, opacity: 0.16 });
  circle(f, { x: f.width / 2 - 22, y: 124, d: 44, fill: C.white });
  text(f, 'P', { x: f.width / 2 - 8, y: 134, size: 22, weight: 'Bold', color: C.primaryDark });
  text(f, 'PsicoManager', { x: 0, y: 196, size: 24, weight: 'Bold', color: C.white, width: f.width, align: 'CENTER' });
  text(f, 'Seu espaço de cuidado, com privacidade.', { x: 0, y: 230, size: 14, color: '#DCEDEA', width: f.width, align: 'CENTER' });

  // card de login
  card(f, 24, 300, f.width - 48, 420, 24);
  text(f, 'Entrar', { x: 48, y: 332, size: 20, weight: 'Bold', color: C.text });

  text(f, 'E-mail', { x: 48, y: 376, size: 12, weight: 'Semi Bold', color: C.muted });
  rect(f, { x: 48, y: 396, w: f.width - 96, h: 50, fill: C.bg, radius: 12, stroke: C.border });
  text(f, 'ana.laura@email.com', { x: 64, y: 412, size: 14, color: C.text });

  text(f, 'Senha', { x: 48, y: 462, size: 12, weight: 'Semi Bold', color: C.muted });
  rect(f, { x: 48, y: 482, w: f.width - 96, h: 50, fill: C.bg, radius: 12, stroke: C.border });
  text(f, '••••••••', { x: 64, y: 500, size: 14, color: C.text });
  text(f, '👁', { x: f.width - 76, y: 498, size: 16 });

  button(f, 'Entrar', 48, 552, f.width - 96, 52, { shadow: true });
  text(f, 'Esqueci minha senha', { x: 0, y: 620, size: 13, weight: 'Semi Bold', color: C.primary, width: f.width, align: 'CENTER' });

  // selo de segurança (responde à dor de privacidade dos pacientes)
  rect(f, { x: 48, y: 658, w: f.width - 96, h: 44, fill: C.primaryLight, radius: 12 });
  text(f, '🔒 Dados protegidos · Conforme a LGPD', { x: 0, y: 672, size: 12, weight: 'Semi Bold', color: C.primaryDark, width: f.width, align: 'CENTER' });

  text(f, 'Criar conta', { x: 0, y: 756, size: 14, weight: 'Semi Bold', color: C.text, width: f.width, align: 'CENTER' });
  return f;
}

// =====================================================================
//  TELA 6 — AGENDAMENTO ONLINE (Paciente)
// =====================================================================
function screenAgendarPaciente(x, y) {
  const f = phone('06 · Paciente — Agendar', x, y);
  text(f, 'Agendar sessão', { x: 24, y: 58, size: 22, weight: 'Bold', color: C.text });
  text(f, 'Escolha o melhor horário para você', { x: 24, y: 90, size: 13, color: C.muted });

  // profissional
  card(f, 24, 124, f.width - 48, 80, 16);
  circle(f, { x: 40, y: 142, d: 44, fill: C.primaryLight });
  text(f, 'LM', { x: 51, y: 156, size: 15, weight: 'Bold', color: C.primaryDark });
  text(f, 'Lívia Mesquita', { x: 96, y: 142, size: 15, weight: 'Semi Bold', color: C.text });
  text(f, 'Psicologia Clínica · TCC', { x: 96, y: 162, size: 12, color: C.muted });
  text(f, 'R$ 250', { x: f.width - 92, y: 152, size: 15, weight: 'Bold', color: C.primary });

  // seleção de dia
  text(f, 'Maio 2026', { x: 24, y: 224, size: 14, weight: 'Semi Bold', color: C.text });
  const ds = [['Seg', '18'], ['Ter', '19'], ['Qua', '20'], ['Qui', '21'], ['Sex', '22']];
  ds.forEach(function (d, i) {
    const dx = 24 + i * 68;
    const on = i === 2;
    rect(f, { x: dx, y: 252, w: 58, h: 72, fill: on ? C.primary : C.surface, radius: 14, stroke: on ? null : C.border, shadow: on });
    text(f, d[0], { x: dx, y: 266, size: 11, color: on ? '#DCEDEA' : C.muted, width: 58, align: 'CENTER' });
    text(f, d[1], { x: dx, y: 286, size: 18, weight: 'Bold', color: on ? C.white : C.text, width: 58, align: 'CENTER' });
  });

  // horários
  text(f, 'Horários disponíveis', { x: 24, y: 344, size: 14, weight: 'Semi Bold', color: C.text });
  const hs = [['08:00', true], ['09:00', false], ['11:00', true], ['14:00', true], ['15:00', true], ['16:00', false]];
  hs.forEach(function (h, i) {
    const col = i % 3, row = Math.floor(i / 3);
    const hx = 24 + col * 116, hy = 372 + row * 60;
    const sel = i === 3;
    const free = h[1];
    rect(f, { x: hx, y: hy, w: 104, h: 48, fill: sel ? C.primary : (free ? C.surface : C.bg), radius: 12, stroke: sel ? null : C.border });
    text(f, h[0], { x: hx, y: hy + 15, size: 14, weight: 'Semi Bold', color: sel ? C.white : (free ? C.text : C.muted), width: 104, align: 'CENTER' });
  });

  // pacote recorrente (pedido dos pacientes: previsibilidade)
  rect(f, { x: 24, y: 504, w: f.width - 48, h: 52, fill: C.primaryLight, radius: 12 });
  rect(f, { x: f.width - 84, y: 518, w: 44, h: 24, fill: C.primary, radius: 12 });
  circle(f, { x: f.width - 62, y: 520, d: 20, fill: C.white });
  text(f, 'Repetir toda quarta às 14h', { x: 40, y: 521, size: 13, weight: 'Semi Bold', color: C.primaryDark });

  // regra 24h (requisito da Lívia)
  rect(f, { x: 24, y: 568, w: f.width - 48, h: 56, fill: C.warningBg, radius: 12 });
  text(f, '⏱ Cancelamentos e reagendamentos só são permitidos com no mínimo 24h de antecedência.',
    { x: 40, y: 580, size: 12, color: C.text, width: f.width - 80, lineHeight: 18 });

  button(f, 'Confirmar agendamento', 24, 642, f.width - 48, 54, { shadow: true });
  bottomNav(f, 'Agenda');
  return f;
}

// =====================================================================
//  TELA 7 — DIÁRIO DE EMOÇÕES (Paciente) — RF8
// =====================================================================
function screenDiario(x, y) {
  const f = phone('07 · Paciente — Diário de Emoções', x, y);
  text(f, 'Diário de emoções', { x: 24, y: 58, size: 22, weight: 'Bold', color: C.text });
  text(f, 'Como você está se sentindo hoje?', { x: 24, y: 90, size: 13, color: C.muted });

  // seletor de humor
  const moods = [['😄', 'Ótimo', C.success], ['🙂', 'Bem', C.primary], ['😐', 'Neutro', C.accent], ['😔', 'Triste', C.warning], ['😣', 'Ansioso', C.danger]];
  moods.forEach(function (m, i) {
    const mx = 24 + i * 68;
    const on = i === 1;
    rect(f, { x: mx, y: 124, w: 58, h: 78, fill: on ? C.primaryLight : C.surface, radius: 16, stroke: on ? null : C.border, shadow: on });
    text(f, m[0], { x: mx, y: 140, size: 24, width: 58, align: 'CENTER' });
    text(f, m[1], { x: mx, y: 178, size: 10, weight: on ? 'Semi Bold' : 'Regular', color: on ? C.primaryDark : C.muted, width: 58, align: 'CENTER' });
  });

  // anotação
  text(f, 'Registrar pensamentos', { x: 24, y: 224, size: 14, weight: 'Semi Bold', color: C.text });
  rect(f, { x: 24, y: 252, w: f.width - 48, h: 120, fill: C.surface, radius: 14, stroke: C.border });
  text(f, 'Hoje consegui aplicar a respiração antes da reunião. Me senti mais no controle...',
    { x: 40, y: 268, size: 13, color: C.text, width: f.width - 80, lineHeight: 20 });

  // tags
  ['Trabalho', 'Sono', 'Família', '+ tag'].forEach(function (t, i) {
    const tx = 24 + i * 86;
    rect(f, { x: tx, y: 388, w: 78, h: 32, fill: i === 3 ? C.bg : C.primaryLight, radius: 16, stroke: i === 3 ? C.border : null });
    text(f, t, { x: tx, y: 396, size: 11, weight: 'Semi Bold', color: i === 3 ? C.muted : C.primaryDark, width: 78, align: 'CENTER' });
  });

  button(f, 'Salvar registro', 24, 436, f.width - 48, 50, { shadow: true });

  // histórico da semana (gráfico simples de humor)
  card(f, 24, 502, f.width - 48, 168, 16);
  text(f, 'Sua semana', { x: 44, y: 522, size: 14, weight: 'Bold', color: C.text });
  const week = [['S', 0.6, C.primary], ['T', 0.8, C.success], ['Q', 0.5, C.accent], ['Q', 0.7, C.primary], ['S', 0.4, C.warning], ['S', 0.85, C.success], ['D', 0.65, C.primary]];
  const wbY = 636, wMax = 70;
  week.forEach(function (w, i) {
    const wx = 48 + i * 44;
    const wh = wMax * w[1];
    rect(f, { x: wx, y: wbY - wh, w: 22, h: wh, fill: w[2], radius: 6 });
    text(f, w[0], { x: wx - 4, y: wbY + 6, size: 10, color: C.muted, width: 30, align: 'CENTER' });
  });

  bottomNav(f, 'Diário');
  return f;
}

// =====================================================================
//  TELA 8 — TELECONSULTA + DOCUMENTOS (Paciente) — RF3, RF10, RF6
// =====================================================================
function screenTeleDocs(x, y) {
  const f = phone('08 · Paciente — Início', x, y);
  text(f, 'Olá, Ana 👋', { x: 24, y: 58, size: 22, weight: 'Bold', color: C.text });
  text(f, 'Sua próxima sessão é hoje', { x: 24, y: 90, size: 13, color: C.muted });

  // card de próxima sessão com link seguro de videochamada
  rect(f, { x: 24, y: 124, w: f.width - 48, h: 168, fill: C.primaryDark, radius: 20 });
  rect(f, { x: 24, y: 124, w: f.width - 48, h: 168, fill: C.primary, opacity: 0.45, radius: 20 });
  pill(f, 'HOJE · 14:00', 44, 148, C.white, C.primaryDark);
  text(f, 'Teleconsulta com', { x: 44, y: 192, size: 13, color: '#DCEDEA' });
  text(f, 'Lívia Mesquita', { x: 44, y: 212, size: 19, weight: 'Bold', color: C.white });
  button(f, '🎥 Entrar na sala segura', 44, 246, f.width - 88, 44, { fill: C.white, textColor: C.primaryDark, shadow: true });

  // lembrete discreto (pedido dos pacientes)
  rect(f, { x: 24, y: 308, w: f.width - 48, h: 48, fill: C.primaryLight, radius: 12 });
  text(f, '🔔 Lembrete discreto: "Compromisso às 14h" (sem citar terapia).',
    { x: 40, y: 318, size: 11, color: C.primaryDark, width: f.width - 80, lineHeight: 16 });

  // central de documentos
  text(f, 'Meus documentos', { x: 24, y: 376, size: 15, weight: 'Bold', color: C.text });
  text(f, 'Ver todos', { x: f.width - 96, y: 378, size: 12, weight: 'Semi Bold', color: C.primary });
  const docs = [
    ['📄 Recibo · Abril', 'Emitido em 30/04', C.successBg],
    ['📎 Exercício: Registro de pensamentos', 'Enviado pela Lívia', C.primaryLight],
    ['📄 Recibo · Março', 'Emitido em 31/03', C.successBg]
  ];
  docs.forEach(function (d, i) {
    const dy = 408 + i * 64;
    rect(f, { x: 24, y: dy, w: f.width - 48, h: 54, fill: C.surface, radius: 12, stroke: C.border });
    rect(f, { x: 24, y: dy, w: 40, h: 54, fill: d[2], radius: 12 });
    text(f, d[0], { x: 76, y: dy + 11, size: 13, weight: 'Semi Bold', color: C.text, width: f.width - 130 });
    text(f, d[1], { x: 76, y: dy + 30, size: 11, color: C.muted });
    text(f, '↓', { x: f.width - 60, y: dy + 16, size: 18, color: C.primary });
  });

  // cobranças (RF7) — paciente só vê o que foi liberado (RN4)
  rect(f, { x: 24, y: 612, w: f.width - 48, h: 56, fill: C.warningBg, radius: 12 });
  text(f, 'Cobrança em aberto', { x: 40, y: 624, size: 12, weight: 'Semi Bold', color: C.text });
  text(f, 'Sessão 20/05 · R$ 250', { x: 40, y: 642, size: 11, color: C.muted });
  button(f, 'Pagar com Pix', f.width - 152, 624, 116, 36, { size: 12 });

  bottomNav(f, 'Início');
  return f;
}

// =====================================================================
//  TÍTULO / CAPA do quadro
// =====================================================================
function header(x, y) {
  const f = figma.createFrame();
  f.name = '00 · PsicoManager — Protótipos';
  f.x = x; f.y = y;
  f.resize(2680, 120);
  f.fills = solid(C.bg);
  text(f, 'PsicoManager', { x: 0, y: 18, size: 40, weight: 'Bold', color: C.primaryDark });
  text(f, 'Protótipos de telas · Plataforma de Gestão Clínica para Psicólogos e Pacientes', { x: 0, y: 72, size: 16, color: C.muted });
  return f;
}

// =====================================================================
//  EXECUÇÃO
// =====================================================================
(async function () {
  try {
    await loadFonts();

    const created = [];
    created.push(header(0, -180));

    // Linha 1 — módulo psicólogo (desktop)
    const gapX = 120, rowY1 = 0, dW = 1280;
    created.push(screenDashboard(0, rowY1));
    created.push(screenAgenda(dW + gapX, rowY1));
    created.push(screenProntuario((dW + gapX) * 2, rowY1));
    created.push(screenFinanceiro((dW + gapX) * 3, rowY1));

    // Linha 2 — módulo paciente (mobile)
    const rowY2 = 832 + 160, pW = 390, pGap = 80;
    created.push(screenLogin(0, rowY2));
    created.push(screenAgendarPaciente(pW + pGap, rowY2));
    created.push(screenDiario((pW + pGap) * 2, rowY2));
    created.push(screenTeleDocs((pW + pGap) * 3, rowY2));

    figma.currentPage.selection = created;
    figma.viewport.scrollAndZoomIntoView(created);
    figma.notify('✅ 8 telas do PsicoManager geradas com sucesso!');
  } catch (e) {
    figma.notify('Erro: ' + e.message);
    console.error(e);
  } finally {
    figma.closePlugin();
  }
})();
