namespace PsicoManager.Infrastructure.Persistence;

/// <summary>
/// DDL auxiliar aplicado nas migrations. RNF04 — a trigger impede UPDATE e
/// DELETE na tabela log_auditoria, garantindo imutabilidade no nível do banco.
/// </summary>
public static class MigrationSql
{
    public const string LogAuditoriaTriggerUp = @"
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
";

    public const string LogAuditoriaTriggerDown = @"
DROP TRIGGER IF EXISTS trg_log_auditoria_imutavel ON log_auditoria;
DROP FUNCTION IF EXISTS fn_bloquear_alteracao_log();
";
}
