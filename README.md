# Zeno

> *Foco sem ruído.*

Zeno é um aplicativo desktop de produtividade leve e elegante, construído para quem quer gerenciar o dia com clareza e intenção. O nome é uma homenagem a **Zenão de Cítio**, fundador do Estoicismo, filosofia de disciplina, foco e equilíbrio.

---


## Stack

| Camada | Tecnologia |
|--------|-----------|
| UI | Avalonia UI 12 (MVVM) |
| Runtime | .NET 8 |
| Banco | SQLite |
| ORM | Dapper |
| MVVM | CommunityToolkit.Mvvm |
| Testes | xUnit + SQLite in-memory |

**Plataformas:** Linux, Windows, macOS

---

## Funcionalidades

### Hoje
Lista de tarefas do dia com input rápido, checkbox animado, badge de prioridade (Alta/Média/Baixa) e painel lateral de edição com notas, data de vencimento e projeto associado.

### Próximos
Tarefas futuras agrupadas por data com labels inteligentes ("Amanhã", "Esta semana") e painel de edição inline.

### Projetos
Crie projetos com nome e cor customizável. Associe tarefas a projetos e acompanhe o contador de pendências. Clique em um projeto para ver e gerenciar suas tarefas.

### Pomodoro
Timer circular animado com ciclos de 25 minutos de foco, 5 minutos de pausa curta e 15 minutos de pausa longa. Muda de cor conforme o estado e persiste ao navegar entre páginas.

### Hidratação
Gauge circular animado para acompanhar a ingestão de água diária. Meta configurável (6, 8 ou 10 copos) com frases que mudam a cada sessão.

### Estatísticas *(em desenvolvimento)*
Dashboard com progresso semanal, streak de dias consecutivos, gráfico de barras dos últimos 7 dias e contador de sessões Pomodoro.

---
