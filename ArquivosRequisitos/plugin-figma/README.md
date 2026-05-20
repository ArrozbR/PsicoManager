# PsicoManager — Gerador de Telas no Figma

Script de plugin que gera automaticamente **8 telas** dos protótipos do PsicoManager
diretamente no seu arquivo do Figma, com base nos requisitos levantados (BPMN,
entrevistas, sistema análogo e formulários dos pacientes).

## Telas geradas

**Módulo Psicólogo (Desktop / Web — 1280×832)**
1. Dashboard — sessões do dia, métricas, alertas de no-show e inadimplência
2. Agenda Dinâmica — visão semanal + bloqueio inteligente (almoço/férias)
3. Prontuário Eletrônico — evolução, anexos, modelos de documentos, trava de edição
4. Financeiro — relatório mensal, gráfico, baixa manual via Pix

**Módulo Paciente (Mobile / App — 390×844)**
5. Acesso / Login — com selo de privacidade (LGPD)
6. Agendamento Online — regra de 24h, pacote recorrente
7. Diário de Emoções — seletor de humor + histórico
8. Início (Teleconsulta + Documentos) — sala segura, lembretes discretos, recibos

## Como rodar (passo a passo)

1. Abra o **Figma Desktop** (precisa ser o app de desktop, não o navegador).
2. Crie um arquivo novo ou abra o do projeto.
3. No menu superior: **Figma → Plugins → Development → New plugin…**
4. Escolha **"Run once"** ou **"New plugin"** e em seguida **"Figma design"**.
5. Quando ele criar a pasta, **substitua** o conteúdo de `manifest.json` e `code.js`
   pelos arquivos deste pacote.
   - (Atalho: ao criar, escolha a opção que pede a pasta e aponte para esta pasta.)
6. Rode em **Plugins → Development → PsicoManager - Gerador de Telas**.
7. As 8 telas aparecem no canvas e o zoom ajusta automaticamente. ✅

## Rastreabilidade (RF → Tela)

| Requisito | Tela |
|-----------|------|
| RF1 Agenda Dinâmica | 1, 2 |
| RF2 Prontuário | 3 |
| RF3 Teleconsulta | 8 |
| RF4 Gestão Financeira | 1, 4 |
| RF5 Comunicação Automatizada | 1, 8 |
| RF6 Painel do Paciente | 8 |
| RF7 Financeiro do Cliente | 8 |
| RF8 Diário de Emoções | 7 |
| RF9 Agendamento Online | 6 |
| RF10 Central de Documentos | 8 |
| RN2 Sem duplo agendamento | 2 |
| RN4 Paciente só vê cobrança liberada | 8 |
| RNF Confidencialidade / Integridade | 3 |

## Observação sobre a fonte

O script usa **Inter** porque é a única fonte garantida em qualquer instalação do
Figma — assim ele roda sem erro. Depois de gerar, você pode trocar a tipografia
no próprio Figma (selecionar tudo → mudar a fonte) caso queira um visual mais autoral.
