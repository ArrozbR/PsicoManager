-- =====================================================================
-- PsicoManager — Schema do banco clínico (PostgreSQL)
-- Gerado a partir do EOBD (Entrega Parte III). Aplique com:
--   psql -U postgres -d psicomanager -f db/schema.sql
-- Alternativa: gerar migration EF Core (ver README, seção "Banco de dados").
-- =====================================================================

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- ---------- psicologo ----------
CREATE TABLE IF NOT EXISTS psicologo (
    id              UUID PRIMARY KEY,
    nome            VARCHAR(150) NOT NULL,
    crp             VARCHAR(10)  NOT NULL,
    email           VARCHAR(255) NOT NULL,
    telefone        VARCHAR(20),
    senha_hash      VARCHAR(255) NOT NULL,
    mfa_ativo       BOOLEAN NOT NULL DEFAULT FALSE,
    ativo           BOOLEAN NOT NULL DEFAULT TRUE,
    criado_em       TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_psicologo_crp   UNIQUE (crp),
    CONSTRAINT uk_psicologo_email UNIQUE (email)
);

-- ---------- paciente ----------
CREATE TABLE IF NOT EXISTS paciente (
    id               UUID PRIMARY KEY,
    psicologo_id     UUID NOT NULL REFERENCES psicologo(id),
    nome             VARCHAR(150) NOT NULL,
    cpf              VARCHAR(11),
    data_nascimento  DATE,
    email            VARCHAR(255),
    telefone         VARCHAR(20),
    whatsapp         VARCHAR(20),
    ativo            BOOLEAN NOT NULL DEFAULT TRUE,
    criado_em        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_paciente_psi_email UNIQUE (psicologo_id, email)
);

-- ---------- vinculo_terapeutico (RN1 / sigilo) ----------
CREATE TABLE IF NOT EXISTS vinculo_terapeutico (
    id            UUID PRIMARY KEY,
    psicologo_id  UUID NOT NULL REFERENCES psicologo(id),
    paciente_id   UUID NOT NULL REFERENCES paciente(id),
    ativo         BOOLEAN NOT NULL DEFAULT TRUE,
    encerrado_em  TIMESTAMPTZ,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS ix_vinculo_psi_pac_ativo
    ON vinculo_terapeutico (psicologo_id, paciente_id, ativo);

-- ---------- sessao ----------
CREATE TABLE IF NOT EXISTS sessao (
    id                   UUID PRIMARY KEY,
    psicologo_id         UUID NOT NULL REFERENCES psicologo(id),
    paciente_id          UUID NOT NULL REFERENCES paciente(id),
    data_hora            TIMESTAMPTZ NOT NULL,
    duracao_min          SMALLINT NOT NULL DEFAULT 50,
    tipo                 VARCHAR(15) NOT NULL,
    status               VARCHAR(15) NOT NULL,
    motivo_cancelamento  TEXT,
    criado_em            TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    atualizado_em        TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS ix_sessao_psi_data ON sessao (psicologo_id, data_hora);

-- ---------- horario_agenda ----------
CREATE TABLE IF NOT EXISTS horario_agenda (
    id            UUID PRIMARY KEY,
    psicologo_id  UUID NOT NULL REFERENCES psicologo(id),
    data          DATE NOT NULL,
    hora_inicio   TIME NOT NULL,
    duracao_min   SMALLINT NOT NULL DEFAULT 50,
    tipo          VARCHAR(15) NOT NULL DEFAULT 'disponivel',
    recorrencia   VARCHAR(15) NOT NULL DEFAULT 'unica',
    data_termino  DATE,
    sessao_id     UUID,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------- prontuario_clinico ----------
CREATE TABLE IF NOT EXISTS prontuario_clinico (
    id                  UUID PRIMARY KEY,
    paciente_id         UUID NOT NULL REFERENCES paciente(id),
    psicologo_id        UUID NOT NULL REFERENCES psicologo(id),
    status              VARCHAR(15) NOT NULL,
    ultima_evolucao_em  TIMESTAMPTZ,
    criado_em           TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_prontuario_paciente UNIQUE (paciente_id)
);

-- ---------- evolucao_clinica ----------
CREATE TABLE IF NOT EXISTS evolucao_clinica (
    id             UUID PRIMARY KEY,
    prontuario_id  UUID NOT NULL REFERENCES prontuario_clinico(id),
    sessao_id      UUID,
    psicologo_id   UUID NOT NULL REFERENCES psicologo(id),
    texto_enc      TEXT NOT NULL,            -- AES-256 em repouso (RNF02)
    data_hora      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    status         VARCHAR(15) NOT NULL,
    prazo_trava_h  SMALLINT NOT NULL DEFAULT 24,
    travar_em      TIMESTAMPTZ NOT NULL,
    criado_em      TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------- anexo ----------
CREATE TABLE IF NOT EXISTS anexo (
    id           UUID PRIMARY KEY,
    evolucao_id  UUID NOT NULL REFERENCES evolucao_clinica(id),
    nome         VARCHAR(255) NOT NULL,
    tipo         VARCHAR(50)  NOT NULL,
    url_storage  VARCHAR(512) NOT NULL,
    tamanho      BIGINT NOT NULL,
    criado_em    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- ---------- cobranca ----------
CREATE TABLE IF NOT EXISTS cobranca (
    id               UUID PRIMARY KEY,
    sessao_id        UUID NOT NULL REFERENCES sessao(id),
    psicologo_id     UUID NOT NULL REFERENCES psicologo(id),
    paciente_id      UUID NOT NULL REFERENCES paciente(id),
    valor            NUMERIC(10,2) NOT NULL,
    status           VARCHAR(15) NOT NULL,
    forma_pagamento  VARCHAR(10) NOT NULL,
    pix_codigo       VARCHAR(512),
    pix_validade     TIMESTAMPTZ,
    data_lancamento  DATE NOT NULL DEFAULT CURRENT_DATE,
    data_pagamento   DATE,
    criado_em        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_cobranca_sessao      UNIQUE (sessao_id),
    CONSTRAINT ck_cobranca_valor_pos   CHECK (valor > 0)
);

-- ---------- notificacao ----------
CREATE TABLE IF NOT EXISTS notificacao (
    id               UUID PRIMARY KEY,
    sessao_id        UUID NOT NULL REFERENCES sessao(id),
    destinatario_id  UUID NOT NULL,
    tipo_dest        VARCHAR(15) NOT NULL,
    tipo             VARCHAR(40) NOT NULL,
    canal            VARCHAR(15) NOT NULL,
    status           VARCHAR(10) NOT NULL,
    tentativas       SMALLINT NOT NULL DEFAULT 0,
    agendado_para    TIMESTAMPTZ NOT NULL,
    enviado_em       TIMESTAMPTZ,
    criado_em        TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_notificacao_tentativas CHECK (tentativas <= 3)
);

-- ---------- teleconsulta ----------
CREATE TABLE IF NOT EXISTS teleconsulta (
    id                UUID PRIMARY KEY,
    sessao_id         UUID NOT NULL REFERENCES sessao(id),
    link_acesso       VARCHAR(255) NOT NULL,
    status            VARCHAR(20) NOT NULL,
    duracao_real_min  SMALLINT,
    link_expira_em    TIMESTAMPTZ NOT NULL,
    criado_em         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_teleconsulta_sessao UNIQUE (sessao_id),
    CONSTRAINT uk_teleconsulta_link   UNIQUE (link_acesso)
);

-- ---------- diario_emocoes ----------
CREATE TABLE IF NOT EXISTS diario_emocoes (
    id             UUID PRIMARY KEY,
    paciente_id    UUID NOT NULL REFERENCES paciente(id),
    compartilhado  BOOLEAN NOT NULL DEFAULT FALSE,
    criado_em      TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uk_diario_paciente UNIQUE (paciente_id)
);

-- ---------- registro_emocao ----------
CREATE TABLE IF NOT EXISTS registro_emocao (
    id            UUID PRIMARY KEY,
    diario_id     UUID NOT NULL REFERENCES diario_emocoes(id),
    data_hora     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    humor         SMALLINT NOT NULL,
    texto_enc     TEXT,
    editavel_ate  TIMESTAMPTZ NOT NULL,
    criado_em     TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT ck_registro_humor CHECK (humor >= 1 AND humor <= 10)
);

-- ---------- log_auditoria (RNF04 — imutável) ----------
CREATE TABLE IF NOT EXISTS log_auditoria (
    id           UUID PRIMARY KEY,
    usuario_id   UUID NOT NULL,
    perfil       VARCHAR(15) NOT NULL,
    acao         VARCHAR(20) NOT NULL,
    entidade     VARCHAR(30) NOT NULL,
    entidade_id  UUID NOT NULL,
    data_hora    TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    ip_origem    VARCHAR(45) NOT NULL,
    resultado    VARCHAR(10) NOT NULL,
    criado_em    TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS ix_log_entidade ON log_auditoria (entidade_id);

-- RNF04 — trigger de imutabilidade: bloqueia UPDATE/DELETE no log
CREATE OR REPLACE FUNCTION fn_bloquear_alteracao_log()
RETURNS TRIGGER AS $$
BEGIN
    RAISE EXCEPTION 'log_auditoria é imutável (RNF04): operação % não permitida.', TG_OP;
END;
$$ LANGUAGE plpgsql;

DROP TRIGGER IF EXISTS trg_log_auditoria_imutavel ON log_auditoria;
CREATE TRIGGER trg_log_auditoria_imutavel
BEFORE UPDATE OR DELETE ON log_auditoria
FOR EACH ROW EXECUTE FUNCTION fn_bloquear_alteracao_log();
