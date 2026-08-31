# Regras do plano de estudos

## Regra 9 - Fluxo geral do motor de planejamento

O planejamento deve separar comportamento normal de intervenÃ§Ã£o adaptativa:

```text
ConfiguraÃ§Ã£o inicial
    â†’ Gerador do plano
    â†’ ExecuÃ§Ã£o
    â†’ ConsequÃªncias do estudo
    â†’ AdaptaÃ§Ã£o futura
```

### Plano base

O gerador distribui automaticamente os tÃ³picos respeitando matÃ©rias
selecionadas, prioridade, tempo por tÃ³pico e disponibilidade diÃ¡ria. O aluno
nÃ£o precisa montar o calendÃ¡rio manualmente.

### ConsequÃªncias da conclusÃ£o

Ao concluir um tÃ³pico, ele sai da fila de conteÃºdo novo e pode gerar:

- uma revisÃ£o futura, conforme o intervalo escolhido pelo usuÃ¡rio;
- questÃµes futuras vinculadas Ã  matÃ©ria e ao tÃ³pico estudado.

QuestÃµes nÃ£o entram no mesmo dia da primeira exposiÃ§Ã£o por padrÃ£o.

### AdaptaÃ§Ã£o

Quando surgirem sinais reais de dificuldade, como muitos erros ou baixa
retenÃ§Ã£o, o sistema pode antecipar revisÃµes, aumentar questÃµes e deslocar
temporariamente conteÃºdo novo. Somente o futuro Ã© reorganizado; o histÃ³rico do
que jÃ¡ ocorreu nunca Ã© alterado.

Quando o domÃ­nio melhorar, revisÃµes e questÃµes extras sÃ£o espaÃ§adas e o tempo
volta a ser destinado a novos tÃ³picos.

## Regra 8 - AdaptaÃ§Ã£o automÃ¡tica por domÃ­nio

ConclusÃ£o do estudo inicial nÃ£o significa domÃ­nio permanente. Desempenho em
questÃµes e retenÃ§Ã£o devem ajustar continuamente o plano, inclusive para
tÃ³picos jÃ¡ concluÃ­dos.

- Queda de desempenho ou retenÃ§Ã£o reduz o intervalo entre revisÃµes.
- O sistema aumenta questÃµes e reforÃ§o daquele tÃ³pico.
- Essas atividades podem consumir temporariamente o tempo destinado a novos
  tÃ³picos.
- Melhora do domÃ­nio espaÃ§a revisÃµes, reduz questÃµes extras e libera tempo para
  avanÃ§ar no conteÃºdo novo.

```text
ConcluÃ­do   = terminou o estudo inicial
Desempenho  = consegue aplicar o conhecimento
RetenÃ§Ã£o    = continua lembrando do conhecimento
```

Esses estados sÃ£o independentes: um nÃ£o implica automaticamente os outros.

## Regra 7 - DivisÃ£o do tempo disponÃ­vel

O tempo diÃ¡rio disponÃ­vel Ã© dividido entre Estudo, RevisÃ£o e QuestÃµes por
percentuais definidos para o ciclo.

DivisÃ£o padrÃ£o:

```text
Estudo    50%
RevisÃ£o   25%
QuestÃµes  25%
Total    100%
```

- O sistema oferece essa divisÃ£o como padrÃ£o.
- O usuÃ¡rio pode personalizar os percentuais.
- A soma dos percentuais deve ser sempre exatamente 100%.
- Os percentuais formam orÃ§amentos diÃ¡rios para cada tipo de atividade.
- A divisÃ£o nunca deve gerar tempo ocioso enquanto houver conteÃºdo pendente.
- Se um orÃ§amento nÃ£o tiver conteÃºdo disponÃ­vel, o tempo pode ser redistribuÃ­do
  para outra atividade pendente.

## Regra 6 - Prioridade adaptativa das matÃ©rias

A afinidade determina a prioridade inicial de cada matÃ©ria. Conforme o sistema
acumula dados reais de desempenho e retenÃ§Ã£o, o plano pode adaptar a
frequÃªncia da matÃ©ria e o tipo de atividade apresentado.

```text
Prioridade = quanto a matÃ©ria deve aparecer
Necessidade = o que o aluno precisa fazer agora
Plano       = teoria, questÃµes, revisÃ£o ou reforÃ§o
```

- Bom desempenho e boa retenÃ§Ã£o mantÃªm o avanÃ§o normal em novos tÃ³picos.
- Baixo desempenho ou baixa retenÃ§Ã£o aumentam revisÃµes, questÃµes e reforÃ§o.
- O avanÃ§o de conteÃºdo novo pode ser reduzido temporariamente quando houver
  dificuldade.
- A prioridade original da matÃ©ria nÃ£o Ã© necessariamente alterada pelos
  dados adaptativos.

A afinidade inicia o ciclo; os dados reais tornam o plano adaptativo.

## Regra 5 - Preenchimento do tempo diário e saldo do tópico

O sistema deve utilizar todo o tempo disponível do dia enquanto houver conteúdo pendente.

- Se o próximo tópico exigir mais tempo que o restante do dia, ele é iniciado com o tempo disponível.
- O tempo que não couber fica como saldo pendente daquela passagem.
- O saldo entra no cálculo do próximo dia de estudo.
- O saldo deve ser concluído antes de iniciar uma nova passagem do mesmo tópico.
- Ao completar o tempo total da passagem, o usuário informa se concluiu o tópico.
- Se concluiu, sai da fila de estudo e poderá retornar como revisão.
- Se não concluiu, permanece em andamento e poderá receber nova passagem conforme o ciclo.

O tempo disponível não deve ser desperdiçado e o saldo de uma passagem nunca deve ser perdido.

## Regra 3 - Definição do tempo planejado

O tempo do tópico usa esta hierarquia:

```text
Tempo específico do tópico
        ↓ se não existir
Tempo específico da matéria
        ↓ se não existir
Tempo padrão da rotina
```

O valor mais específico sempre vence. A carga representa o tempo total para
concluir o tópico; subtópicos não acrescentam tempo.

## Regra 1 — Planejamento no nível de tópico

O plano de estudos é gerado no nível de tópico (`SyllabusNode`).

- Apenas tópicos de primeiro nível geram blocos de estudo.
- Subtópicos pertencem ao tópico pai.
- Subtópicos não geram blocos próprios.
- Subtópicos não acrescentam tempo automaticamente.
- O tempo configurado para o tópico cobre também seus subtópicos.
- O progresso e a conclusão são controlados pelo tópico principal.

Exemplo:

```text
Português
└── Concordância verbal       ← gera bloco e recebe tempo
    ├── Concordância nominal  ← não gera bloco
    └── Casos especiais        ← não gera bloco
```

Se o tópico tiver duas horas configuradas, o planejamento terá duas horas para
esse tópico, independentemente da quantidade de subtópicos.

## Regra 2 — Seleção e ordem inicial dos tópicos

Entram no plano somente:

- tópicos das matérias selecionadas pelo usuário;
- tópicos ainda não concluídos;
- tópicos respeitando a ordem cadastrada no edital.

A ordem interna dos tópicos de cada matéria não é alterada pela prioridade ou
afinidade nesta primeira versão.

```text
Ordem dentro da matéria → ordem do edital
Frequência da matéria   → prioridade/afinidade
```

A prioridade/afinidade define quantas vezes a matéria aparece no ciclo e sua
frequência de estudo, mas não faz o motor pular a sequência interna de tópicos.
