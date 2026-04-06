# Documentação Legal e Regulatória - PsicoManager

## 1. Leis e Legislações Federais

* **Lei Geral de Proteção de Dados Pessoais (LGPD - Lei nº 13.709/2018):**
    * O sistema armazenará "dados pessoais sensíveis", como informações sobre o histórico emocional e o motivo da busca por terapia dos pacientes. A LGPD exige que a arquitetura do software contemple a privacidade desde a sua concepção, garantindo criptografia, mecanismos claros de consentimento e controle rígido sobre quem tem acesso a essas informações.
* **Marco Civil da Internet (Lei nº 12.965/2014):**
    * Sendo uma plataforma Web operada em nuvem, o Marco Civil estabelece diretrizes sobre a guarda de registros de acesso (logs) por prazos específicos. Isso é fundamental para auditorias e para garantir a responsabilização caso haja algum acesso indevido ao sistema.

## 2. Regulamentações do Conselho Federal de Psicologia (CFP)

* **Código de Ética Profissional do Psicólogo (Resolução CFP nº 10/2005):**
    * O Artigo 9º do código é a base legal para o requisito não funcional de **Confidencialidade**. O sistema deve garantir tecnicamente que os dados do prontuário e das evoluções sejam acessíveis estritamente ao psicólogo responsável, impedindo vazamentos que caracterizariam quebra de sigilo profissional.
* **Resolução CFP nº 01/2009 (com alterações da Resolução CFP nº 05/2010):**
    * Esta resolução dispõe sobre a obrigatoriedade do registro documental (prontuário) decorrente da prestação de serviços psicológicos. Ela é a base jurídica para a regra de negócio que determina que o histórico clínico deve ser mantido por, no mínimo, 5 anos. O sistema deve impedir a exclusão definitiva de prontuários antes desse prazo legal.
* **Resolução CFP nº 11/2018:**
    * Como o projeto prevê a integração de uma sala de videochamada segura e nativa para teleconsultas, esta resolução é crucial. Ela regulamenta a prestação de serviços psicológicos por meio de Tecnologias da Informação e Comunicação, exigindo que a plataforma garanta o sigilo e a adequação técnica das sessões online.

## 3. Padrões de Qualidade de Software (ISO/IEC 25010)

Para assegurar que a plataforma atenda aos requisitos críticos levantados, os seguintes atributos do modelo de qualidade da ISO/IEC 25010 devem ser aplicados:

* **Segurança:**
    * É a característica central do projeto. O sistema deve prover **Confidencialidade** (dados visíveis apenas a perfis autorizados, como a regra de que o paciente nunca pode ver o próprio prontuário), **Integridade** (registros clínicos e financeiros não podem ser alterados indevidamente ou após uma trava de tempo pós-sessão) e **Responsabilização** (rastrear no banco de dados quem executou cada ação).
* **Confiabilidade:**
    * O requisito de **Disponibilidade** foi explicitamente levantado, exigindo acesso remoto estável. A plataforma precisa ter tolerância a falhas para que o profissional e o paciente não fiquem sem acesso à agenda, aos pagamentos ou à teleconsulta no momento marcado.
* **Usabilidade:**
    * Com as demandas de um painel autônomo para o paciente agendar sessões, a interface precisa ter alta **Proteção contra erros do usuário**, como a.