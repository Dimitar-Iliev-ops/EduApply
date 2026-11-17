import { useEffect, useState } from 'react'
import { getJson, type Campaign, type ServiceName } from './api'

type CatalogState =
  | { kind: 'loading' }
  | { kind: 'ready'; campaigns: Campaign[] }
  | { kind: 'unavailable' }

const services: { id: ServiceName; label: string }[] = [
  { id: 'catalog', label: 'Каталог' },
  { id: 'applications', label: 'Заявления' },
  { id: 'ranking', label: 'Класиране' },
]

function ServiceStatus({ service }: { service: (typeof services)[number] }) {
  const [status, setStatus] = useState<'checking' | 'online' | 'offline'>('checking')

  useEffect(() => {
    const controller = new AbortController()
    const signal = AbortSignal.any([controller.signal, AbortSignal.timeout(5000)])
    getJson<{ status: string }>(`/api/${service.id}/health`, signal).then(
      (result) => setStatus(result.status === 'ok' ? 'online' : 'offline'),
      () => { if (!controller.signal.aborted) setStatus('offline') },
    )
    return () => controller.abort()
  }, [service.id])

  return <li><span>{service.label}</span><span className={status}>
    {status === 'online' ? 'Работи' : status === 'offline' ? 'Недостъпна' : 'Проверка…'}
  </span></li>
}

export default function App() {
  const [catalog, setCatalog] = useState<CatalogState>({ kind: 'loading' })
  const [attempt, setAttempt] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    const signal = AbortSignal.any([controller.signal, AbortSignal.timeout(5000)])
    getJson<Campaign[]>('/api/catalog/campaigns', signal).then(
      (campaigns) => setCatalog({ kind: 'ready', campaigns }),
      () => { if (!controller.signal.aborted) setCatalog({ kind: 'unavailable' }) },
    )
    return () => controller.abort()
  }, [attempt])

  return (
    <>
      <header><nav className="wrap" aria-label="Основна навигация">
        <a className="brand" href="#top">EduApply</a>
        <a href="#campaigns">Кампании</a>
        <a href="#next">Следващи модули</a>
      </nav></header>
      <main className="wrap" id="top">
        <section className="intro" aria-labelledby="title">
          <div>
            <p className="eyebrow">ДЕМОНСТРАЦИОННА ВЕРСИЯ</p>
            <h1 id="title">Онлайн прием<br />в училище</h1>
            <p className="lead">Разгледайте примерна кампания и местата в нейните паралелки.</p>
          </div>
          <aside className="panel" aria-labelledby="service-title">
            <h2 id="service-title">Състояние на услугите</h2>
            <ul className="status-list" aria-live="polite">{services.map((service) =>
              <ServiceStatus key={service.id} service={service} />)}</ul>
          </aside>
        </section>
        <section id="campaigns" aria-labelledby="campaign-title">
          <h2 id="campaign-title">Кампании</h2>
          <div className="panel" aria-live="polite">
            {catalog.kind === 'loading' && <p>Зареждане…</p>}
            {catalog.kind === 'unavailable' && <>
              <p>Каталогът е временно недостъпен.</p>
              <button type="button" onClick={() => {
                setCatalog({ kind: 'loading' })
                setAttempt((value) => value + 1)
              }}>Опитай отново</button>
            </>}
            {catalog.kind === 'ready' && catalog.campaigns.length === 0 && <p>Няма кампании.</p>}
            {catalog.kind === 'ready' && catalog.campaigns.map((campaign) =>
              <article key={campaign.id}>
                <p className="eyebrow">{campaign.isDemo ? 'ПРИМЕРНИ ДАННИ' : campaign.schoolYear}</p>
                <h3>{campaign.name}</h3>
                <ul className="program-list">{campaign.programs.map((program) =>
                  <li key={program.name}><span>{program.name}</span><strong>{program.capacity} места</strong></li>)}</ul>
              </article>)}
          </div>
        </section>
        <section id="next" aria-labelledby="next-title">
          <h2 id="next-title">Следващи модули</h2>
          <div className="modules">
            <article className="panel"><p className="eyebrow">ПРЕДСТОИ</p><h3>Заявления</h3>
              <p>Чернова на заявление, подреждане на желания и проверка на документи.</p></article>
            <article className="panel"><p className="eyebrow">ПРЕДСТОИ</p><h3>Класиране</h3>
              <p>Изчисляване на бал, разпределение по места и преглед на резултатите.</p></article>
          </div>
        </section>
      </main>
      <footer className="wrap">EduApply · Дипломен проект</footer>
    </>
  )
}
